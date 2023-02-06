using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq
{
    public interface IRabbitMqUtilities
    {
        void EnsureExists(IModel channel, string address, bool isSender = false);
        (string exchange, string queue, string routingKey) GetExachangeAndQueueNames(string address);
    }
}
