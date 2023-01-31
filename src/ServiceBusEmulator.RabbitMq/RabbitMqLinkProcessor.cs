using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using Xim.Simulators.ServiceBus.Options;
using Xim.Simulators.ServiceBus.Rabbit;
using Xim.Simulators.ServiceBus.Rabbit.Endpoints;

namespace Xim.Simulators.ServiceBus.InMemory
{

    internal class RabbitMqLinkProcessor : ILinkProcessor, IReceiverLinkFinder
    {
        private readonly IServiceProvider _services;
        private readonly ISecurityContext _securityContext;
        private readonly RabbitMqBackendOptions _options;
        private readonly ILogger _logger;
        private readonly RabbitMQ.Client.IConnection _connection;
        private readonly RabbitMqUtilities _utilities;

        private ConcurrentDictionary<string, ListenerLink> OutgoingLinks { get; } = new ConcurrentDictionary<string, ListenerLink>();

        public RabbitMqLinkProcessor(IServiceProvider services, ISecurityContext securityContext, RabbitMqUtilities utilities, IOptions<RabbitMqBackendOptions> options, ILogger<RabbitMqLinkProcessor> logger)
        {
            _services = services;
            _securityContext = securityContext;
            _utilities = utilities;
            _options = options.Value;
            _logger = logger;

            var factory = new RabbitMQ.Client.ConnectionFactory()
            {
                Password = _options.Password,
                UserName = _options.Username,
                HostName = _options.Host,
                Port = _options.Port
            };
            _connection = factory.CreateConnection();
        }

        public void Process(AttachContext attachContext)
        {
            if (string.IsNullOrEmpty(attachContext.Attach.LinkName))
            {
                attachContext.Complete(new Error(ErrorCode.InvalidField) { Description = "Empty link name not allowed." });
                _logger.LogError($"Could not attach empty link to {GetType().Name}.");
                return;
            }

            if (!_securityContext.IsAuthorized(attachContext.Link.Session.Connection))
            {
                attachContext.Complete(new Error(ErrorCode.UnauthorizedAccess) { Description = "Not authorized." });
                _logger.LogError($"Could not attach unathorized link to {GetType().Name}.");
                return;
            }


            if (attachContext.Link.Role)
            {
                AttachIncomingLink(attachContext, (Target)attachContext.Attach.Target);
            }
            else
            {
                AttachOutgoingLink(attachContext, (Source)attachContext.Attach.Source);
            }
        }

        private void AttachIncomingLink(AttachContext attachContext, Target target)
        {
            var channel = _connection.CreateModel();
            _utilities.EnsureExists(channel, target.Address);

            if (target.Address.Contains("$management"))
            {
                var endpoint = _services.GetRequiredService<RabbitMqManagementSenderEndpoint>();
                endpoint.SetContext(channel, target);
                attachContext.Complete(endpoint, 300);
            }
            else
            {
                var endpoint = _services.GetRequiredService<RabbitMqSenderEndpoint>();
                endpoint.SetContext(channel, target);
                attachContext.Complete(endpoint, 300);
            }

            _logger.LogDebug($"Attached incoming link to entity '{target.Address}'.");
        }

        private void AttachOutgoingLink(AttachContext attachContext, Source source)
        {
            var channel = _connection.CreateModel();
            _utilities.EnsureExists(channel, source.Address);

            attachContext.Attach.MaxMessageSize = 9999;

            if (source.Address.Contains("$management"))
            {
                string targetAddress = ((Target)attachContext.Attach.Target).Address;
                OutgoingLinks.TryAdd(targetAddress, attachContext.Link);
                attachContext.Link.Closed += (s, e) => OutgoingLinks.TryRemove(targetAddress, out _);

                var endpoint = _services.GetRequiredService<RabbitMqManagementReceiverEndpoint>();
                endpoint.SetContext(channel);

                attachContext.Complete(endpoint, 0);
            }
            else
            {
                var endpoint = _services.GetRequiredService<RabbitMqReceiverEndpoint>();
                endpoint.SetContext(channel, source, attachContext.Attach.RcvSettleMode);
                attachContext.Complete(endpoint, 0);
            }

            _logger.LogDebug($"Attached outgoing link to queue '{source.Address}'.");
        }

        public ListenerLink FindByAddress(string address)
        {
            if (OutgoingLinks.TryGetValue(address, out var link))
            {
                return link;
            }

            return null;
        }
    }
}
