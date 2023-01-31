using Amqp;
using Amqp.Listener;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ServiceBusEmulator.InMemory.Delivering
{
    public sealed class DeliveryQueue : IDeliveryQueue, IDisposable
    {
        private bool _disposed;
        private long _sequence;

        private readonly BlockingCollection<Delivery> _queue
            = new(new ConcurrentQueue<Delivery>());

        private readonly ConcurrentDictionary<Message, Delivery> _delivery
            = new();

        public void Enqueue(Delivery delivery)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(DeliveryQueue).Name);
            }

            _ = delivery.Message.AddSequenceNumber(Interlocked.Increment(ref _sequence));
            _queue.Add(delivery);
        }

        public Message Dequeue(CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(DeliveryQueue).Name);
            }

            Delivery delivery = _queue.Take(cancellationToken);

            _delivery[delivery.Message] = delivery;

            return delivery.Message;
        }

        public void Process(MessageContext messageContext)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(DeliveryQueue).Name);
            }

            if (_delivery.TryRemove(messageContext.Message, out Delivery delivery))
            {
                delivery.Process(messageContext.DeliveryState);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _queue.Dispose();
        }
    }
}
