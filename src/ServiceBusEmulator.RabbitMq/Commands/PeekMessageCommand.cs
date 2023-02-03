using Amqp;
using Amqp.Types;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;

namespace ServiceBusEmulator.RabbitMq.Commands
{
    public class PeekMessageCommand : IManagementCommand
    {
        private readonly IRabbitMqUtilities _utilities;
        private readonly IRabbitMqMapper _mapper;

        public PeekMessageCommand(IRabbitMqUtilities utilities, IRabbitMqMapper mapper)
        {
            _utilities = utilities;
            _mapper = mapper;
        }

        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {

            (_, string? queueName, _) = _utilities.GetExachangeAndQueue(address);

            Map requestBody = (Map)request.Body;
            int fromSequence = Convert.ToInt32(requestBody[ManagementConstants.Properties.FromSequenceNumber]);
            int messageCount = Convert.ToInt32(requestBody[ManagementConstants.Properties.MessageCount]);

            List messages = new();
            IEnumerable<Map> rabbitMessages = PeekMessagesFromRabbit(channel, queueName, fromSequence, messageCount);
            messages.AddRange(rabbitMessages);

            Map responseBody = new()
            {
                [ManagementConstants.Properties.Messages] = messages
            };

            return (new Message(responseBody), messages.Any() ? AmqpResponseStatusCode.OK : AmqpResponseStatusCode.NoContent);
        }

        private IEnumerable<Map> PeekMessagesFromRabbit(IModel channel, string queueName, int fromSequence, int messageCount)
        {
            List<ulong> deliveryTags = new();
            try
            {
                BasicGetResult rabbitGetResult;
                do
                {
                    rabbitGetResult = channel.BasicGet(queueName, autoAck: false);

                    if (rabbitGetResult == null)
                    {
                        break;
                    }

                    deliveryTags.Add(rabbitGetResult.DeliveryTag);

                    if (fromSequence-- > 1)
                    {
                        continue;
                    }

                    yield return GetMessageItem(rabbitGetResult);

                    messageCount--;
                }
                while (messageCount > 0);
            }
            finally
            {
                foreach (ulong deliveyTag in deliveryTags)
                {
                    channel.BasicReject(deliveyTag, requeue: true);
                }
            }
        }

        private Map GetMessageItem(BasicGetResult rabbitGetResult)
        {
            Message message = new();
            _mapper.MapFromRabbit(message, rabbitGetResult.Body, rabbitGetResult.BasicProperties);
            ByteBuffer buffer = message.Encode();

            Map map = new()
            {
                [ManagementConstants.Properties.Message] = buffer.Buffer[0..buffer.Length].ToArray()
            };

            return map;
        }
    }
}
