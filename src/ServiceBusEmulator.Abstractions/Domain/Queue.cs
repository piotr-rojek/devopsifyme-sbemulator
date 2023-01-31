using System;
using System.Text.RegularExpressions;

namespace Xim.Simulators.ServiceBus.Model
{
    /// <summary>
    /// Describes a service bus queue.
    /// </summary>
    public class Queue
    {
        // The name can contain only letters, numbers, periods, hyphens, underscores, tildes, slashes and backward slashes.
        // The name must start and end with a letter or number.
        // The name must be between 1 and 260 characters long.
        private static readonly Regex RxValidName = new Regex("^[A-Za-z0-9]$|^[A-Za-z0-9][\\w\\.\\-\\/~]{0,258}[A-Za-z0-9]$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the name of the <see cref="Queue"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="Queue"/> class.
        /// </summary>
        /// <param name="name">The name of the queue.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is not valid.</exception>
        public Queue(string name)
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
