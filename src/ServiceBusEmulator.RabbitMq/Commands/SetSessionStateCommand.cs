using Amqp;
using RabbitMQ.Client;
using Xim.Simulators.ServiceBus.Azure;

namespace Xim.Simulators.ServiceBus.Rabbit.Management
{
    public class SetSessionStateCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            return (new Message(null), AmqpResponseStatusCode.OK);
        }
    }
}
