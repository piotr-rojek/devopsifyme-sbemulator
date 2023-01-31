using Amqp;
using Amqp.Types;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Azure;

namespace ServiceBusEmulator.RabbitMq.Commands
{
    public class RenewSessionLockCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            Map requestBody = (Map)request.Body;
            _ = requestBody[ManagementConstants.Properties.LockTokens] as Guid[];
            Map responseBody = new()
            {
                [ManagementConstants.Properties.Expiration] = DateTime.UtcNow.AddMinutes(5)
            };

            return (new Message(responseBody), AmqpResponseStatusCode.OK);
        }
    }
}
