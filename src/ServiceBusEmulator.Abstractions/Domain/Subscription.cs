using System.Text.RegularExpressions;

namespace ServiceBusEmulator.Abstractions.Domain
{
    /// <summary>
    /// Describes a service bus subscription.
    /// </summary>
    public class Subscription
    {
        // The name can contain only letters, numbers, periods, hyphens and underscores.
        // The name must start and end with a letter or number.
        // The name must be between 1 and 50 characters long.
        private static readonly Regex RxValidName = new("^[A-Za-z0-9]$|^[A-Za-z0-9][\\w\\.\\-]{0,48}[A-Za-z0-9]$", RegexOptions.Compiled);

        /// <summary>
        /// The name of the <see cref="Subscription"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="name">The name of the subscription.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is not valid.</exception>
        public Subscription(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!RxValidName.IsMatch(name))
            {
                throw new ArgumentException(null, nameof(name));
            }

            Name = name;
        }
    }
}