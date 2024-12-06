namespace RabbitMqSafePublish;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var rabbitMqPublisher = new RabbitMqPublisher(new RabbitMqConfig
        {
            HostName = "localhost",
            Username = "guest",
            Password = "guest",
            Port = 5672,
            VirtualHost = "/"
        });

        var published = await rabbitMqPublisher.PublishAsync<object>("Test-Ex", "Test-RK", new { Name = "Ali" });

        Console.WriteLine($"published => {published}");
        Console.ReadLine();
    }
}