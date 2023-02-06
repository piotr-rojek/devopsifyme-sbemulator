using RabbitMQ.Client;

namespace ServiceBusEmulator.RabbitMq
{
    public class RabbitMqUtilities : IRabbitMqUtilities
    {
        public (string exchange, string queue, string routingKey) GetExachangeAndQueueNames(string address)
        {
            if (address.StartsWith("/"))
            {
                address = address[1..];
            }

            string[] parts = address.Split('/');
            string exchangeName = parts[0];
            string routingKey = parts[0];
            string queueName = address.Contains("/Subscriptions/") ? $"{parts[0]}-sub-{parts[2]}" : parts[0];

            if (address.EndsWith("$deadletterqueue"))
            {
                (string? dlqQueue, string? dlqRoutingKey) = GetDlqQueueNames(queueName);
                queueName = dlqQueue ?? queueName;
                routingKey = dlqRoutingKey ?? routingKey;
            }

            return (exchangeName, queueName, routingKey);
        }

        protected (string? dlqQueue, string? dlqRoutingKey) GetDlqQueueNames(string queue)
        {
            if(queue.EndsWith("dlq"))
            {
                return (null, null);
            }

            return ($"{queue}-dlq", $"{queue}-dlq");
        }

        public void EnsureExists(IModel channel, string address, bool isSender = false)
        {
            (string exchange, string queue, string routingKey) = GetExachangeAndQueueNames(address);
            (string? dlqName, string? dlqRoutingKey) = GetDlqQueueNames(queue);

            channel.ExchangeDeclare(exchange, "topic", true, false);

            if (!isSender)
            {
                var queueArguments = new Dictionary<string, object>();

                if (dlqName != null && dlqRoutingKey != null)
                {   
                    // create DLQ queues and configure routing
                    queueArguments["x-dead-letter-exchange"] = exchange;
                    queueArguments["x-dead-letter-routing-key"] = dlqRoutingKey;

                    _ = channel.QueueDeclare(dlqName, true, false, false);
                    channel.QueueBind(dlqName, exchange, dlqRoutingKey);
                }

                _ = channel.QueueDeclare(queue, true, false, false, queueArguments);
                channel.QueueBind(queue, exchange, routingKey);
            }
        }
    }
}
