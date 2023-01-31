using Amqp;
using Amqp.Types;
using RabbitMQ.Client;
using System;
using Xim.Simulators.ServiceBus.Azure;

namespace Xim.Simulators.ServiceBus.Rabbit.Management
{
    public class RenewSessionLockCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            var requestBody = (Map)request.Body;

            var tokens = requestBody[ManagementConstants.Properties.LockTokens] as Guid[];
            var responseBody = new Map
            {
                [ManagementConstants.Properties.Expiration] = DateTime.UtcNow.AddMinutes(5)
            };

            return (new Message(responseBody), AmqpResponseStatusCode.OK);
        }
    }
}
