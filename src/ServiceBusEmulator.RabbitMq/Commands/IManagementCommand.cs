using Amqp;
using RabbitMQ.Client;
using Xim.Simulators.ServiceBus.Azure;

namespace Xim.Simulators.ServiceBus.Rabbit.Management
{
    public interface IManagementCommand
    {
        (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address);
    }
}
