using Amqp;
using Amqp.Types;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;

namespace ServiceBusEmulator.RabbitMq.Commands
{
    public class GetSessionStateCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            Map responseBody = new()
            {
                [ManagementConstants.Properties.SessionState] = new byte[0]
            };

            return (new Message(responseBody), AmqpResponseStatusCode.OK);
        }
    }
}
