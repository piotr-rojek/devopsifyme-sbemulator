using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Options;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public class RabbitMqInitializer : IRabbitMqInitializer
    {
        private readonly ServiceBusEmulatorOptions _options;
        private readonly IRabbitMqUtilities _utilities;
        private bool _initialized;

        public RabbitMqInitializer(IRabbitMqUtilities utilities, IOptions<ServiceBusEmulatorOptions> options)
        {
            _options = options.Value;
            _utilities = utilities;
        }

        public void Initialize(IModel channel)
        {
            if (_initialized)
            {
                return;
            }

            string[] entities = _options.QueuesAndTopics?.Split(',', ';') ?? new string[0];
            foreach (string entity in entities)
            {
                _utilities.EnsureExists(channel, entity);
            }

            _initialized = true;
        }
    }
}
