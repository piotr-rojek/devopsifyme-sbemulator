using RabbitMQ.Client;
using System.Collections.Generic;

namespace Xim.Simulators.ServiceBus.Rabbit
{
    public class RabbitMqUtilities
    {
        public (string exchange, string queue, string routingKey) GetExachangeAndQueue(string address)
        {
            if (address.StartsWith("/"))
            {
                address = address.Substring(1);
            }

            var parts = address.Split('/');
            var exchangeName = parts[0];
            var queueName = string.Empty;
            var routingKey = string.Empty;

            if (address.Contains("/Subscriptions/"))
            {
                queueName = $"{parts[0]}-sub-{parts[2]}";
            }
            else
            {
                queueName = parts[0];
            }

            return(exchangeName, queueName, routingKey);    
        }

        public void EnsureExists(IModel channel, string address)
        {
            (var exchange, var queue, var routingKey) = GetExachangeAndQueue(address);

            channel.ExchangeDeclare(exchange, "fanout", true, false);
            channel.QueueDeclare(queue, true, false, false);
            channel.QueueBind(queue, exchange, routingKey, new Dictionary<string, object>());
        }
    }
}
