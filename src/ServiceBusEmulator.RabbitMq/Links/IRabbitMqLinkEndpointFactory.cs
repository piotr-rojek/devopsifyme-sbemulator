using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public interface IRabbitMqLinkEndpointFactory
    {
        (LinkEndpoint endpoint, int initialCredit) CreateEndpoint(IModel channel, Attach attach, bool isSender);
    }
}
