using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqSafePublish.Exceptions;

namespace RabbitMqSafePublish;

public class RabbitMqPublisher
{
    private readonly TimeSpan _publishTimeout = TimeSpan.FromSeconds(30);
    private readonly RabbitMqConfig _rabbitMqConfig;
    private TaskCompletionSource<bool> _pendingConfirmation;

    public RabbitMqPublisher(RabbitMqConfig rabbitMqConfig)
    {
        _rabbitMqConfig = rabbitMqConfig;
    }

    public async Task<bool> PublishAsync<TMessage>(string exchange, string routingKey, TMessage message)
    {
        var publishSuccess = false;
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqConfig.HostName,
            Port = _rabbitMqConfig.Port,
            UserName = _rabbitMqConfig.Username,
            Password = _rabbitMqConfig.Password,
            VirtualHost = _rabbitMqConfig.VirtualHost
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.BasicReturn += OnBasicReturn;
        channel.BasicAcks += OnAcknowledged;
        channel.BasicNacks += OnNotAcknowledged;
        channel.ModelShutdown += OnModelShutdown;

        channel.ConfirmSelect();
        var basicProperties = channel.CreateBasicProperties();
        basicProperties.Persistent = true;

        try
        {
            _pendingConfirmation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange, routingKey, true, basicProperties, body);
            var onlyAcksReceived = channel.WaitForConfirms(_publishTimeout, out var timedOut);
            publishSuccess = await _pendingConfirmation.Task && onlyAcksReceived && !timedOut;
        }
        catch (Exception ex)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine(ex);
            }
        }

        return publishSuccess;
    }

    private void OnBasicReturn(object model, BasicReturnEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Body.Span);
        var exception = new MessageReturnedException(body, args.Exchange, args.RoutingKey, args.ReplyCode, args.ReplyText);
        _pendingConfirmation.TrySetException(exception);
    }

    private void OnAcknowledged(object model, BasicAckEventArgs args)
    {
        _pendingConfirmation.TrySetResult(true);
    }

    private void OnNotAcknowledged(object model, BasicNackEventArgs args)
    {
        var exception = new MessageNotAcknowledgedException("The message was not acknowledged by RabbitMQ Publisher");
        _pendingConfirmation.TrySetException(exception);
    }

    private void OnModelShutdown(object model, ShutdownEventArgs args)
    {
        if (model is not null)
        {
            ((IModel)model).ModelShutdown -= OnModelShutdown;
            ((IModel)model).BasicAcks -= OnAcknowledged;
            ((IModel)model).BasicNacks -= OnNotAcknowledged;
            ((IModel)model).BasicReturn -= OnBasicReturn;
        }

        var exception = new MessageNotConfirmedException(args.ToString());
        _pendingConfirmation.TrySetException(exception);
    }
}