namespace Xim.Simulators.ServiceBus.InMemory.Delivering
{
    /// <summary>
    /// Represents a delivery result of a posted message.
    /// </summary>
    public enum DeliveryResult
    {
        /// <summary>
        /// Message posted for processing.
        /// </summary>
        Posted,

        /// <summary>
        /// Delivery completed successfully.
        /// </summary>
        Completed,

        /// <summary>
        /// Delivery abandoned.
        /// </summary>
        Abandoned,

        /// <summary>
        /// Delivery failed and message dead-lettered.
        /// </summary>
        DeadLettered,

        /// <summary>
        /// Message lost.
        /// </summary>
        Lost,

        /// <summary>
        /// Delivery unknown.
        /// </summary>
        Unknown
    }
}