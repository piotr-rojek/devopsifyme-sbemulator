using System;
using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;

namespace Xim.Simulators.ServiceBus.Rabbit.Endpoints
{
    public class RabbitMqSenderEndpoint : LinkEndpoint
    {
        private readonly RabbitMqUtilities _utilities;
        private readonly RabbitMqMapper _mapper;

        public RabbitMqSenderEndpoint(RabbitMqMapper mapper, RabbitMqUtilities utilities)
        {
            _mapper = mapper;
            _utilities = utilities;
        }

        protected IModel Channel { get; private set; }

        public Target Target { get; private set; }

        protected string ExchangeName { get; private set; }

        protected string QueueName { get; private set; }

        protected string RoutingKey { get; private set; }

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
            var prop = Channel.CreateBasicProperties();
            byte[] body = _mapper.MapToRabbit(prop, message);

            Channel.BasicPublish(ExchangeName, RoutingKey, true, prop, body);
        }
    }
}
