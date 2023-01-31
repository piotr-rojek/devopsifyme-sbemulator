using System.Collections.Generic;
using Xim.Simulators.ServiceBus.InMemory.Delivering;

namespace Xim.Simulators.ServiceBus.InMemory.Entities
{
    /// <summary>
    /// Represents a service bus topic.
    /// </summary>
    public interface ITopic
    {
        /// <summary>
        /// Get the name of the topic.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the deliveries associated with the topic.
        /// </summary>
        IReadOnlyList<ITopicDelivery> Deliveries { get; }

        /// <summary>
        /// Gets the topic subscriptions.
        /// </summary>
        IReadOnlyDictionary<string, IQueue> Subscriptions { get; }

        /// <summary>
        /// Posts a message to the topic.
        /// </summary>
        /// <param name="message">The <see cref="Amqp.Message"/> to post to the topic.</param>
        /// <returns>The <see cref="ITopicDelivery"/> representing the delivery state of the message.</returns>
        ITopicDelivery Post(Amqp.Message message);
    }
}
