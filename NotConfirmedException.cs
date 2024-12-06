namespace RabbitMqSafePublish;

public class MessageReturnedException : Exception
{
    public MessageReturnedException(string reasonText, int reasonCode, string payload, string exchange,
        string routingKey, string queue)
    {
        ReasonText = reasonText;
        ReasonCode = reasonCode;
        Payload = payload;
        Exchange = exchange;
        RoutingKey = routingKey;
        Queue = queue;
    }

    public string ReasonText { get; }
    public int ReasonCode { get; }
    public string Payload { get; }
    public string Exchange { get; }
    public string RoutingKey { get; }
    public string Queue { get; }

    public override string ToString()
    {
        var errorInfo = new
        {
            ErrorMessage = "message can not routed, please bind queue to exchange",
            ReasonCode,
            ReasonText,
            Exchange,
            RoutingKey,
            Queue
        };

        return $"{base.ToString()}, ErrorInfo => {errorInfo}";
    }
}