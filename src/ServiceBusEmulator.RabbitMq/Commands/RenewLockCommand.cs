using Amqp;
using Amqp.Types;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;

namespace ServiceBusEmulator.RabbitMq.Commands
{
    public class RenewLockCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            Map requestBody = (Map)request.Body;

            Guid[]? tokens = requestBody[ManagementConstants.Properties.LockTokens] as Guid[];
            Map responseBody = new()
            {
                [ManagementConstants.Properties.Expirations] = Enumerable.Repeat(DateTime.UtcNow.AddMinutes(5), tokens?.Length ?? 0).ToArray()
            };

            return (new Message(responseBody), AmqpResponseStatusCode.OK);
        }
    }
}
