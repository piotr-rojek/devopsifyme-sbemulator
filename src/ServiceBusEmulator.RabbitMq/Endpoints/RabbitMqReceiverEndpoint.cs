using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public class RabbitMqReceiverEndpoint : LinkEndpointWithSourceContext
    {
        private readonly IRabbitMqDeliveryTagTracker _deliveryTagTracker;
        private readonly IRabbitMqMapper _mapper;
        private readonly IRabbitMqUtilities _utilities;

        public RabbitMqReceiverEndpoint(IRabbitMqDeliveryTagTracker deliveryTagTracker, IRabbitMqMapper mapper, IRabbitMqUtilities utilities)
        {
            _deliveryTagTracker = deliveryTagTracker;
            _mapper = mapper;
            _utilities = utilities;
        }

        public IModel Channel { get; private set; } = null!;

        protected string QueueName { get; private set; } = null!;

        protected ReceiverSettleMode ReceiveSettleMode { get; private set; }

        public override void SetContext(IModel channel, Source source, ReceiverSettleMode rcvSettleMode)
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
            ulong deliveryTag = _deliveryTagTracker[dispositionContext.Message.DeliveryTag] 
                ?? throw new Exception("Delivery tag not found");

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
                var message = GetAmqpMessage(e, ref requestedCount);
                flowContext.Link.SendMessage(message);
                _deliveryTagTracker[message.DeliveryTag] = e.DeliveryTag;
            };

            Channel.BasicQos(0, 1, false);
            _ = Channel.BasicConsume(QueueName, ReceiveSettleMode == ReceiverSettleMode.First, consumer);
        }

        protected Message? GetAmqpMessage(BasicDeliverEventArgs e, ref int requestedCount)
        {
            Interlocked.Decrement(ref requestedCount);
            if (requestedCount <= 0)
            {
                Channel.BasicCancel(e.ConsumerTag);
            }

            Message message = new();
            _mapper.MapFromRabbit(message, e.Body, e.BasicProperties);

            return message;
        }

        public override void OnMessage(MessageContext messageContext)
        {
            throw new NotImplementedException();
        }
    }
}
