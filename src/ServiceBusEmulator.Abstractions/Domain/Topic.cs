using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Xim.Simulators.ServiceBus.Model
{
    /// <summary>
    /// Describes a service bus topic.
    /// </summary>
    public class Topic
    {
        // The name can contain only letters, numbers, periods, hyphens, underscores, tildes and slashes.
        // The name must start and end with a letter or number.
        // The name must be between 1 and 260 characters long.
        private static readonly Regex RxValidName = new Regex("^[A-Za-z0-9]$|^[A-Za-z0-9][\\w\\.\\-\\/~]{0,258}[A-Za-z0-9]$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the name of the topic.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the collection of topic subscriptions.
        /// </summary>
        public IReadOnlyList<Subscription> Subscriptions { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Topic"/> class.
        /// </summary>
        /// <param name="name">The name of the <see cref="Topic"/>.</param>
        /// <param name="subscriptions">An array of <see cref="Subscription"/> that contains zero
        /// or more subscriptions.</param>
        public Topic(string name, params Subscription[] subscriptions)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!RxValidName.IsMatch(name))
            {
                throw new ArgumentException(null, nameof(name));
            }

            if (subscriptions?.Any(s => s == null) == true)
            {
                throw new ArgumentException(null, nameof(subscriptions));
            }

            if (subscriptions?
                .Select(s => s.Name)
                .GroupBy(n => n, StringComparer.InvariantCultureIgnoreCase)
                .FirstOrDefault(g => g.Count() > 1) is IGrouping<string, string> duplicate)
            {
                throw new ArgumentException(null, nameof(subscriptions));
            }

            Name = name;
            Subscriptions = (subscriptions ?? Array.Empty<Subscription>()).AsReadOnly();
        }
    }
}
