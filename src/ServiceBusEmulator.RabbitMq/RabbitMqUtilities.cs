using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq
{
    public class RabbitMqUtilities
    {
        public (string exchange, string queue, string routingKey) GetExachangeAndQueue(string address)
        {
            if (address.StartsWith("/"))
            {
                address = address[1..];
            }

            string[] parts = address.Split('/');
            string exchangeName = parts[0];
            string routingKey = string.Empty;

            string queueName = address.Contains("/Subscriptions/") ? $"{parts[0]}-sub-{parts[2]}" : parts[0];

            return (exchangeName, queueName, routingKey);
        }

        public void EnsureExists(IModel channel, string address, bool isSender = false)
        {
            (string? exchange, string? queue, string? routingKey) = GetExachangeAndQueue(address);

            channel.ExchangeDeclare(exchange, "fanout", true, false);

            if(isSender)
            {
                _ = channel.QueueDeclare(queue, true, false, false);
                channel.QueueBind(queue, exchange, routingKey, new Dictionary<string, object>());
            }
        }
    }
}
