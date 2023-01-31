using System;
using System.Collections.Generic;
using System.Linq;
using Amqp;
using Xim.Simulators.ServiceBus.InMemory.Delivering;

namespace Xim.Simulators.ServiceBus.InMemory.Entities
{
    internal sealed class TopicEntity : ITopic, IEntity, IDisposable
    {
        private bool _disposed;
        private readonly List<TopicDelivery> _deliveries = new List<TopicDelivery>();

        public string Name { get; }

        public IReadOnlyList<ITopicDelivery> Deliveries { get; }

        public IReadOnlyDictionary<string, IQueue> Subscriptions { get; }

        internal TopicEntity(string name, IEnumerable<string> subscriptions)
        {
            Name = name;
            Subscriptions = subscriptions
                .Select(subscription => (IQueue)new QueueEntity(subscription))
                .ToDictionary(subscription => subscription.Name, StringComparer.OrdinalIgnoreCase)
                .AsReadOnly();
            Deliveries = _deliveries.AsReadOnly();
        }

        public ITopicDelivery Post(Message message)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(TopicEntity).Name);
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            var delivery = new TopicDelivery(message, PostToSubscriptions(message));
            _deliveries.Add(delivery);
            return delivery;
        }

        public override string ToString()
            => Name;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            foreach (IQueue subscription in Subscriptions.Values)
            {
                ((QueueEntity)subscription).Dispose();
            }

            foreach (TopicDelivery delivery in _deliveries)
            {
                delivery.Dispose();
            }
        }

        private IReadOnlyList<IDelivery> PostToSubscriptions(Message message)
            => Subscriptions
                .Values
                .Select(subscription => subscription.Post(message.Clone()))
                .ToArray();

        void IEntity.Post(Message message) => Post(message);

        DeliveryQueue IEntity.DeliveryQueue => null;
    }
}