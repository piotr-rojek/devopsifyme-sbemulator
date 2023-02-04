using System.Collections.Concurrent;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public class RabbitMqDeliveryTagTracker : IRabbitMqDeliveryTagTracker
    {
        private readonly ConcurrentDictionary<Guid, ulong> _deliveryTags = new();

        public ulong? this[byte[] amqpDeliveryTag]
        {
            get => _deliveryTags.TryGetValue(new Guid(amqpDeliveryTag), out var value)
                ? value
                : null;

            set => _ = value == null
                ? _deliveryTags.TryRemove(new Guid(amqpDeliveryTag), out _)
                : _deliveryTags.TryAdd(new Guid(amqpDeliveryTag), value.Value);
        }
    }
}
