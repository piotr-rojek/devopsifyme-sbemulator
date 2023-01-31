using Amqp.Framing;
using Amqp.Listener;
using ServiceBusEmulator.InMemory.Entities;

namespace ServiceBusEmulator.InMemory.Endpoints
{


    internal sealed class IncomingLinkEndpoint : LinkEndpoint
    {
        private readonly IEntity _entity;

        internal IncomingLinkEndpoint(IEntity entity)
        {
            _entity = entity;
        }

        public override void OnLinkClosed(ListenerLink link, Error error)
        {
            base.OnLinkClosed(link, error);
        }

        public override void OnMessage(MessageContext messageContext)
        {
            _entity.Post(messageContext.Message.Clone());
            messageContext.Complete();
        }

        public override void OnFlow(FlowContext flowContext)
        {
        }

        public override void OnDisposition(DispositionContext dispositionContext)
        {
        }
    }
}
