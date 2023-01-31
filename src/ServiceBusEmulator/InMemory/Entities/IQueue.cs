using System.Collections.Generic;
using Xim.Simulators.ServiceBus.InMemory.Delivering;

namespace Xim.Simulators.ServiceBus.InMemory.Entities
{
    /// <summary>
    /// Represents a service bus queue.
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// Gets the name of the queue.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the deliveries associated with the queue.
        /// </summary>
        IReadOnlyList<IDelivery> Deliveries { get; }

        /// <summary>
        /// Posts a message to the queue.
        /// </summary>
        /// <param name="message">The <see cref="Amqp.Message"/> to post to the queue.</param>
        /// <returns>The <see cref="IDelivery"/> representing the processing state of the message.</returns>
        IDelivery Post(Amqp.Message message);
    }
}
