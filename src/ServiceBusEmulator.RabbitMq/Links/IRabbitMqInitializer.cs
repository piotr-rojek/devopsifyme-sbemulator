using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public interface IRabbitMqInitializer
    {
        void Initialize(IModel channel);
    }
}
