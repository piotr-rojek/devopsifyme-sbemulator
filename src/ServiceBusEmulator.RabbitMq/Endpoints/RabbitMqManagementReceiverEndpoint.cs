using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public class RabbitMqManagementReceiverEndpoint : LinkEndpointWithSourceContext
    {
        public IModel Channel { get; private set; } = null!;

        public override void SetContext(IModel channel, Source source, ReceiverSettleMode settleMode)
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
