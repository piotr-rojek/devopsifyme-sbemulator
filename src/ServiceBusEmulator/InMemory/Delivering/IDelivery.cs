using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp.Framing;

namespace Xim.Simulators.ServiceBus.InMemory.Delivering
{
    /// <summary>
    /// Provides message delivery information and means to wait for queue delivery.
    /// </summary>
    public interface IDelivery
    {
        /// <summary>
        /// Gets the posted <see cref="Amqp.Message"/>.
        /// </summary>
        Amqp.Message Message { get; }

        /// <summary>
        /// Gets the date and time in UTC the message was posted.
        /// </summary>
        DateTime Posted { get; }

        /// <summary>
        /// Gets the date and time in UTC the message was processed or null if still processing.
        /// </summary>
        DateTime? Processed { get; }

        /// <summary>
        /// Gets the <see cref="DeliveryState"/> of the message.
        /// </summary>
        DeliveryState State { get; }

        /// <summary>
        /// Gets the <see cref="DeliveryResult"/> of the message.
        /// </summary>
        DeliveryResult? Result { get; }

        /// <summary>
        /// Waits for the <see cref="Message"/> to be delivered asynchronously.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> period to wait for the delivery.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>True if message processed; false otherwise.</returns>
        /// <exception cref="OperationCanceledException">If <see cref="CancellationToken"/> cancelled.</exception>
        Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    }
}
