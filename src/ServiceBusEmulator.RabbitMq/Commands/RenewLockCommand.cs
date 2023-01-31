using Amqp;
using Amqp.Types;
using RabbitMQ.Client;
using System;
using System.Linq;
using Xim.Simulators.ServiceBus.Azure;

namespace Xim.Simulators.ServiceBus.Rabbit.Management
{
    public class RenewLockCommand : IManagementCommand
    {
        public (Message, AmqpResponseStatusCode) Handle(Message request, IModel channel, string address)
        {
            var requestBody = (Map)request.Body;

            var tokens = requestBody[ManagementConstants.Properties.LockTokens] as Guid[];
            var responseBody = new Map
            {
                [ManagementConstants.Properties.Expirations] = Enumerable.Repeat(DateTime.UtcNow.AddMinutes(5), tokens.Length).ToArray()
            };

            return (new Message(responseBody), AmqpResponseStatusCode.OK);
        }
    }
}
