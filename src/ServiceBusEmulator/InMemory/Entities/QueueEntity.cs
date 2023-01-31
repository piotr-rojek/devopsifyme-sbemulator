using Amqp;
using ServiceBusEmulator.InMemory.Delivering;
using System;
using System.Collections.Generic;

namespace ServiceBusEmulator.InMemory.Entities
{
    internal sealed class QueueEntity : IQueue, IEntity, IDisposable
    {
        private bool _disposed;
        private readonly DeliveryQueue _deliveryQueue = new();
        private readonly List<Delivery> _deliveries = new();

        public string Name { get; }

        public IReadOnlyList<IDelivery> Deliveries { get; }

        internal QueueEntity(string name)
        {
            Name = name;
            Deliveries = _deliveries.AsReadOnly();
        }

        public IDelivery Post(Message message)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(QueueEntity).Name);
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Delivery delivery = new(message);
            _deliveryQueue.Enqueue(delivery);
            _deliveries.Add(delivery);
            return delivery;
        }

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _deliveryQueue.Dispose();

            foreach (Delivery delivery in _deliveries)
            {
                delivery.Dispose();
            }
        }

        void IEntity.Post(Message message)
        {
            _ = Post(message);
        }

        DeliveryQueue IEntity.DeliveryQueue => _deliveryQueue;
    }
}
