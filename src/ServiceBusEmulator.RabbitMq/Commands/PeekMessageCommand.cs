using Amqp;
using Amqp.Types;
using System;
using System.Linq;
using RabbitMQ.Client;
using System.Collections.Generic;
using Xim.Simulators.ServiceBus.Azure;
using System.Threading.Channels;

namespace Xim.Simulators.ServiceBus.Rabbit.Management
{
    public class PeekMessageCommand : IManagementCommand
    {
        private readonly RabbitMqUtilities _utilities;
        private readonly RabbitMqMapper _mapper;

        public PeekMessageCommand(RabbitMqUtilities utilities, RabbitMqMapper mapper)
        {
            _utilities = utilities;
            _mapper = mapper;
        }

        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            
            (_, var queueName, _) = _utilities.GetExachangeAndQueue(address);

            var requestBody = (Map)request.Body;
            int fromSequence = Convert.ToInt32(requestBody[ManagementConstants.Properties.FromSequenceNumber]);
            int messageCount = Convert.ToInt32(requestBody[ManagementConstants.Properties.MessageCount]);

            var messages = new List();
            var rabbitMessages = PeekMessagesFromRabbit(channel, queueName, fromSequence, messageCount);
            messages.AddRange(rabbitMessages);

            var responseBody = new Map
            {
                [ManagementConstants.Properties.Messages] = messages
            };

            return (new Message(responseBody), messages.Any() ? AmqpResponseStatusCode.OK : AmqpResponseStatusCode.NoContent);
        }

        private IEnumerable<Map> PeekMessagesFromRabbit(IModel channel, string queueName, int fromSequence, int messageCount)
        {
            var deliveryTags = new List<ulong>();
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

                    if (fromSequence-- > 1)
                    {
                        continue;
                    }

                    deliveryTags.Add(rabbitGetResult.DeliveryTag);

                    yield return GetMessageItem(rabbitGetResult);

                    messageCount--;
                }
                while (messageCount > 0);
            }
            finally
            {
                foreach (var deliveyTag in deliveryTags)
                {
                    channel.BasicReject(deliveyTag, requeue: true);
                }
            }
        }

        private Map GetMessageItem(BasicGetResult rabbitGetResult)
        {
            var message = new Message();
            _mapper.MapFromRabbit(message, rabbitGetResult.Body, rabbitGetResult.BasicProperties);
            var buffer = message.Encode();

            var map = new Map
            {
                [ManagementConstants.Properties.Message] = buffer.Buffer[0..buffer.Length].ToArray()
            };

            return map;
        }
    }
}
