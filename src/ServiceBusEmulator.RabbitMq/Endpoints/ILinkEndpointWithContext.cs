using Amqp.Framing;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public interface ILinkEndpointWithContext
    {
        void SetContext(IModel channel, Target target);
    }
}
