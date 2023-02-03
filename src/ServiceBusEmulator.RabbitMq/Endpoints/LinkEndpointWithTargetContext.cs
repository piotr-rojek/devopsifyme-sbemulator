using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public abstract class LinkEndpointWithTargetContext : LinkEndpoint
    {
        public abstract void SetContext(IModel channel, Target target);
    }
}
