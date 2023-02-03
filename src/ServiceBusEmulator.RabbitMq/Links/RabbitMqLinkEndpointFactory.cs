using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ServiceBusEmulator.RabbitMq.Endpoints;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public class RabbitMqLinkEndpointFactory : IRabbitMqLinkEndpointFactory
    {
        private readonly IRabbitMqUtilities _utilities;
        private readonly ILogger<RabbitMqLinkEndpointFactory> _logger;
        private readonly IServiceProvider _services;

        public RabbitMqLinkEndpointFactory(IServiceProvider services, IRabbitMqUtilities utilities, ILogger<RabbitMqLinkEndpointFactory> logger)
        {
            _services = services;
            _utilities = utilities;
            _logger = logger;
        }

        public LinkEndpoint CreateSenderLinkEndpoint(IModel channel, Target target)
        {
            _utilities.EnsureExists(channel, target.Address, isSender: true);

            LinkEndpointWithTargetContext linkEndpoint = target.Address.Contains("$management")
                ? _services.GetRequiredService<RabbitMqManagementSenderEndpoint>()
                : _services.GetRequiredService<RabbitMqSenderEndpoint>();

            linkEndpoint.SetContext(channel, target);

            return linkEndpoint;
        }

        public LinkEndpoint CreateReceiverEndpoint(IModel channel, Source source, ReceiverSettleMode rcvSettleMode)
        {
            _utilities.EnsureExists(channel, source.Address);

            LinkEndpointWithSourceContext linkEndpoint = source.Address.Contains("$management")
                ? _services.GetRequiredService<RabbitMqManagementReceiverEndpoint>()
                : _services.GetRequiredService<RabbitMqReceiverEndpoint>();

            linkEndpoint.SetContext(channel, source, rcvSettleMode);

            return linkEndpoint;
        }

        public (LinkEndpoint endpoint, int initialCredit) CreateEndpoint(IModel channel, Attach attach, bool isSender)
        {
            var target = (Target)attach.Target;
            var source = (Source)attach.Source;

            int initialCredit = isSender ? 300 : 0;
            LinkEndpoint linkEndpoint = isSender
                ? CreateSenderLinkEndpoint(channel, target)
                : CreateReceiverEndpoint(channel, source, attach.RcvSettleMode);

            _logger.LogDebug($"Attached link {linkEndpoint} for source '{source.Address}', target '{target.Address}'.");

            return (linkEndpoint, initialCredit);
        }
    }
}
