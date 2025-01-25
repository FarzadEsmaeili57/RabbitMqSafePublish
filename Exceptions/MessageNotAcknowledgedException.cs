namespace RabbitMqSafePublish.Exceptions;

[Serializable]
public class MessageNotAcknowledgedException : Exception
{
    public MessageNotAcknowledgedException()
    {
    }

    public MessageNotAcknowledgedException(string message) : base(message)
    {
    }

    public MessageNotAcknowledgedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}