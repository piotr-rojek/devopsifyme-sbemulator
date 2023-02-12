using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public interface IRabbitMqChannelFactory
    {
        IModel CreateChannel();
    }
}
