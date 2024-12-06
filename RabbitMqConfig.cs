namespace RabbitMqSafePublish
{
    public class RabbitMqConfig
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
    }
}