using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public class RabbitMqManagementReceiverEndpoint : LinkEndpoint
    {
        public IModel Channel { get; private set; } = null!;

        public void SetContext(IModel channel)
        {
            Channel = channel;
        }

        public override void OnDisposition(DispositionContext dispositionContext)
        {
            dispositionContext.Complete();
        }

        public override void OnFlow(FlowContext flowContext)
        {
        }

        public override void OnLinkClosed(ListenerLink link, Error error)
        {
            Channel.Dispose();
        }
    }
}
