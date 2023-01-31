using Amqp;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;

namespace ServiceBusEmulator.RabbitMq.Commands
{
    public class SetSessionStateCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            return (new Message(null), AmqpResponseStatusCode.OK);
        }
    }
}
