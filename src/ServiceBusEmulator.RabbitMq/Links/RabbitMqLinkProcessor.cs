using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Security;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public class RabbitMqLinkProcessor : ILinkProcessor
    {
        private readonly IRabbitMqLinkEndpointFactory _linkEndpointFactory;
        private readonly IRabbitMqLinkRegister _linkRegister;
        private readonly IRabbitMqChannelFactory _channelFactory;
        private readonly IRabbitMqInitializer _initializer;
        private readonly ISecurityContext _securityContext;
        private readonly ILogger _logger;


        public RabbitMqLinkProcessor(IRabbitMqChannelFactory channelFactory,IRabbitMqInitializer intializer, ISecurityContext securityContext, ILogger<RabbitMqLinkProcessor> logger, IRabbitMqLinkRegister linkRegister, IRabbitMqLinkEndpointFactory linkEndpointFactory)
        {
            _channelFactory = channelFactory;
            _initializer = intializer;
            _securityContext = securityContext;
            _logger = logger;
            _linkRegister = linkRegister;
            _linkEndpointFactory = linkEndpointFactory;
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

            IModel channel = _channelFactory.CreateChannel();
            _initializer.Initialize(channel);

            (var linkEndpoint, int initialCredit) = _linkEndpointFactory.CreateEndpoint(channel, attachContext.Attach, attachContext.Link.Role);

            _linkRegister.RegisterLink(((Target)attachContext.Attach.Target).Address, attachContext.Link);

            attachContext.Complete(linkEndpoint, initialCredit);
        }
    }
}
