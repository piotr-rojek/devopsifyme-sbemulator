using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public class RabbitMqSenderEndpoint : LinkEndpoint
    {
        private readonly IRabbitMqUtilities _utilities;
        private readonly IRabbitMqMapper _mapper;

        public RabbitMqSenderEndpoint(IRabbitMqMapper mapper, IRabbitMqUtilities utilities)
        {
            _mapper = mapper;
            _utilities = utilities;
        }

        protected IModel Channel { get; private set; } = null!;

        public Target Target { get; private set; } = null!;

        protected string ExchangeName { get; private set; } = null!;

        protected string QueueName { get; private set; } = null!;

        protected string RoutingKey { get; private set; } = null!;

        public void SetContext(IModel channel, Target target)
        {
            Channel = channel;
            Target = target;
            (ExchangeName, QueueName, RoutingKey) = _utilities.GetExachangeAndQueue(Target.Address);
        }

        public override void OnLinkClosed(ListenerLink link, Error error)
        {
            Channel.Dispose();
        }

        public override void OnDisposition(DispositionContext dispositionContext)
        {
            throw new NotImplementedException();
        }

        public override void OnFlow(FlowContext flowContext)
        {
            throw new NotImplementedException();
        }

        public override void OnMessage(MessageContext messageContext)
        {
            OnMessage(messageContext.Message);
            messageContext.Complete();
        }

        protected void OnMessage(Message message)
        {
            IBasicProperties prop = Channel.CreateBasicProperties();
            byte[] body = _mapper.MapToRabbit(prop, message);

            Channel.BasicPublish(ExchangeName, RoutingKey, true, prop, body);
        }
    }
}
