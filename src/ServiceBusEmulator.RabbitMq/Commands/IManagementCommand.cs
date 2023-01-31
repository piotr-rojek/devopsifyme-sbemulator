using Amqp;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;

namespace ServiceBusEmulator.RabbitMq.Commands
{
    public interface IManagementCommand
    {
        (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address);
    }
}
