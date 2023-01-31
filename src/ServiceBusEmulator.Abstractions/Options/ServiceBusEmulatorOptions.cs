using ServiceBusEmulator.Abstractions.Domain;
using System.Security.Cryptography.X509Certificates;

namespace ServiceBusEmulator.Abstractions.Options
{
    /// <summary>
    /// Service bus simulator settings.
    /// </summary>
    public class ServiceBusEmulatorOptions
    {
        /// <summary>
        /// Returns the <see cref="X509Certificate2"/> used to setup secure links, or null if none set.
        /// </summary>
        public X509Certificate2 ServerCertificate { get; set; } = null!;

        /// <summary>
        /// Loads the <see cref="ServerCertificate"/> from user certificate store.
        /// </summary>
        public string? ServerCertificateThumbprint { get; set; }

        /// <summary>
        /// Loads the <see cref="ServerCertificate"/> from disk.
        /// </summary>
        public string? ServerCertificatePath { get; set; }

        /// <summary>
        /// Password for the <see cref="ServerCertificatePath"/>
        /// </summary>
        public string? ServerCertificatePassword { get; set; }

        /// <summary>
        /// Gets the preferred service bus port.
        /// </summary>
        public int Port { get; set; } = 5671;

        /// <summary>
        /// Gets the list of registered topics with subscriptions.
        /// </summary>
        public IList<Topic> Topics { get; } = new List<Topic>();

        /// <summary>
        /// Gets the list of registered queues.
        /// </summary>
        public IList<Queue> Queues { get; } = new List<Queue>();
    }
}
