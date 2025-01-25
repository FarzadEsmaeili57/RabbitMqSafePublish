namespace RabbitMqSafePublish.Exceptions;

[Serializable]
public class MessageReturnedException : Exception
{
    public string Body { get; }
    public string Exchange { get; }
    public string RoutingKey { get; }
    public ushort ReplyCode { get; }
    public string ReplyText { get; }

    public MessageReturnedException()
    {
    }

    public MessageReturnedException(string message) : base(message)
    {
    }

    public MessageReturnedException(string body, string exchange, string routingKey, ushort replyCode, string replyText)
        : base($"The message was returned by RabbitMQ: {replyCode}-{replyText}")
    {
        Body = body;
        Exchange = exchange;
        RoutingKey = routingKey;
        ReplyCode = replyCode;
        ReplyText = replyText;
    }

    public MessageReturnedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}