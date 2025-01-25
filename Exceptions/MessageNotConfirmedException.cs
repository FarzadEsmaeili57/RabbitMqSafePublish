namespace RabbitMqSafePublish.Exceptions;

[Serializable]
public class MessageNotConfirmedException : Exception
{
    public MessageNotConfirmedException()
    {
    }

    public MessageNotConfirmedException(string reason) : base($"The message was not confirmed: {reason}")
    {
    }

    public MessageNotConfirmedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}