namespace ServiceBusEmulator.RabbitMq.Options
{
    public class RabbitMqBackendOptions
    {
        public string Username { get; set; } = "user";
        public string Password { get; set; } = "password";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
    }
}
