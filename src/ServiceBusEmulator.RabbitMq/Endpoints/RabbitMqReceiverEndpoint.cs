using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public class RabbitMqReceiverEndpoint : LinkEndpoint
    {
        private readonly RabbitMqMapper _mapper;
        private readonly RabbitMqUtilities _utilities;
        private readonly Dictionary<Guid, ulong> _deliveryTags = new();

        public RabbitMqReceiverEndpoint(RabbitMqMapper mapper, RabbitMqUtilities utilities)
        {
            _mapper = mapper;
            _utilities = utilities;
        }

        public IModel Channel { get; private set; } = null!;

        protected string QueueName { get; private set; } = null!;

        protected ReceiverSettleMode ReceiveSettleMode { get; private set; }

        public void SetContext(IModel channel, Source source, ReceiverSettleMode rcvSettleMode)
        {
            Channel = channel;
            ReceiveSettleMode = rcvSettleMode;
            (_, QueueName, _) = _utilities.GetExachangeAndQueue(source.Address);
        }

        public override void OnLinkClosed(ListenerLink link, Error error)
        {
            Channel.Dispose();
        }

        public override void OnDisposition(DispositionContext dispositionContext)
        {
            ulong deliveryTag = _deliveryTags[new Guid(dispositionContext.Message.DeliveryTag)];
            if (dispositionContext.Settled)
            {
                dispositionContext.Complete();
                return;
            }

            switch (dispositionContext.DeliveryState.Descriptor.Name)
            {
                case "amqp:accepted:list":
                    Channel.BasicAck(deliveryTag, false);
                    break;
                case "amqp:released:list":
                    Channel.BasicNack(deliveryTag, false, true);
                    break;
                case "amqp:modified:list":
                    Channel.BasicNack(deliveryTag, false, true);
                    break;
                case "amqp:rejected:list": //deadletter
                    Channel.BasicNack(deliveryTag, false, false);
                    break;
            }

            dispositionContext.Complete();
        }

        public override void OnFlow(FlowContext flowContext)
        {
            int requestedCount = flowContext.Messages;

            EventingBasicConsumer consumer = new(Channel);
            consumer.Received += (m, e) =>
            {
                if (--requestedCount <= 0)
                {
                    Channel.BasicCancel(e.ConsumerTag);
                }

                Message message = new();
                _mapper.MapFromRabbit(message, e.Body, e.BasicProperties);

                flowContext.Link.SendMessage(message);
                _deliveryTags[new Guid(message.DeliveryTag)] = e.DeliveryTag;
            };

            Channel.BasicQos(0, 1, false);
            _ = Channel.BasicConsume(QueueName, ReceiveSettleMode == ReceiverSettleMode.First, consumer);
        }

        public override void OnMessage(MessageContext messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
