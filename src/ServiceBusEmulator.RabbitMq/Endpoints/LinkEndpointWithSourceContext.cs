using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public abstract class LinkEndpointWithSourceContext : LinkEndpoint
    {
        public abstract void SetContext(IModel channel, Source source, ReceiverSettleMode rcvSettleMode);
    }
}
