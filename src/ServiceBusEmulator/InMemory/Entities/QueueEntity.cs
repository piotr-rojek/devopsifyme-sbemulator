using System;
using System.Collections.Generic;
using Amqp;
using Xim.Simulators.ServiceBus.InMemory.Delivering;

namespace Xim.Simulators.ServiceBus.InMemory.Entities
{
    internal sealed class QueueEntity : IQueue, IEntity, IDisposable
    {
        private bool _disposed;
        private readonly DeliveryQueue _deliveryQueue = new DeliveryQueue();
        private readonly List<Delivery> _deliveries = new List<Delivery>();

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
                throw new ObjectDisposedException(typeof(QueueEntity).Name);
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            var delivery = new Delivery(message);
            _deliveryQueue.Enqueue(delivery);
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

            _deliveryQueue.Dispose();

            foreach (Delivery delivery in _deliveries)
            {
                delivery.Dispose();
            }
        }

        void IEntity.Post(Message message) => Post(message);

        DeliveryQueue IEntity.DeliveryQueue => _deliveryQueue;
    }
}
