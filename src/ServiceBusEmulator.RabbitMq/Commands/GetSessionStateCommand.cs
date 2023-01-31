using Amqp;
using Amqp.Types;
using RabbitMQ.Client;
using Xim.Simulators.ServiceBus.Azure;

namespace Xim.Simulators.ServiceBus.Rabbit.Management
{
    public class GetSessionStateCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            var responseBody = new Map
            {
                [ManagementConstants.Properties.SessionState] = new byte[0]
            };

            return (new Message(responseBody), AmqpResponseStatusCode.OK);
        }
    }
}
