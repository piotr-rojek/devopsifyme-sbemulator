using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amqp;

namespace Xim.Simulators.ServiceBus.InMemory.Delivering
{
    internal sealed class TopicDelivery : ITopicDelivery, IDisposable
    {
        private bool _disposed;

        public DateTime Posted { get; }

        public Message Message { get; }

        public IReadOnlyList<IDelivery> Subscriptions { get; }

        internal TopicDelivery(Message message, IReadOnlyList<IDelivery> subscriptions)
        {
            Posted = DateTime.UtcNow;
            Message = message;
            Subscriptions = subscriptions;
        }

        public async Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(TopicDelivery).Name);
            IEnumerable<Task<bool>> subscriptionTasks = Subscriptions
                .Select(s => s.WaitAsync(timeout, cancellationToken));
            var results = await Task.WhenAll(subscriptionTasks).ConfigureAwait(false);
            return results.All(r => r);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            foreach (IDelivery subscription in Subscriptions)
            {
                (subscription as IDisposable)?.Dispose();
            }
        }
    }
}
