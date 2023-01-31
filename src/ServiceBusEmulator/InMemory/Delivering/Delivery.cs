using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;

namespace Xim.Simulators.ServiceBus.InMemory.Delivering
{
    public sealed class Delivery : IDelivery, IDisposable
    {
        private bool _disposed;
        private readonly ManualResetEventSlim _processedEvent = new ManualResetEventSlim(false);

        public Message Message { get; }

        public DateTime Posted { get; }

        public DateTime? Processed { get; private set; }

        public DeliveryState State { get; private set; }

        public DeliveryResult? Result => GetResult(State);

        internal Delivery(Message message)
        {
            Posted = DateTime.UtcNow;
            Message = message;
        }

        private static DeliveryResult? GetResult(DeliveryState deliveryState)
        {
            if (deliveryState == null)
                return null;
            else if (deliveryState is Rejected)
                return DeliveryResult.DeadLettered;
            else if (deliveryState is Modified)
                return DeliveryResult.Abandoned;
            else if (deliveryState is Accepted)
                return DeliveryResult.Completed;
            else if (deliveryState is Released)
                return DeliveryResult.Lost;
            else
                return DeliveryResult.Unknown;
        }

        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
            => Task.Run(() =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = _processedEvent.Wait(timeout, cancellationToken);
                    return _disposed
                        ? Task.FromException<bool>(new ObjectDisposedException(nameof(Delivery)))
                        : Task.FromResult(result);
                }
                catch (OperationCanceledException)
                {
                    return Task.FromCanceled<bool>(cancellationToken);
                }
            });

        internal void Process(DeliveryState deliveryState)
        {
            Processed = DateTime.UtcNow;
            State = deliveryState;
            _processedEvent.Set();
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _processedEvent.Set();
            _processedEvent.Dispose();
        }
    }
}
