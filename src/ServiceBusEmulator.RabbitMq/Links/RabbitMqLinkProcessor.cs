using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ServiceBusEmulator.Abstractions.Security;
using ServiceBusEmulator.RabbitMq.Options;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public class RabbitMqLinkProcessor : ILinkProcessor
    {
        private readonly RabbitMqBackendOptions _options;
        private readonly IRabbitMqLinkEndpointFactory _linkEndpointFactory;
        private readonly IRabbitMqLinkRegister _linkRegister;
        private readonly ISecurityContext _securityContext;
        private readonly ILogger _logger;

        private RabbitMQ.Client.IConnection? _connection;
        private RabbitMQ.Client.ConnectionFactory _connectionFactory;

        public RabbitMqLinkProcessor(ISecurityContext securityContext, IOptions<RabbitMqBackendOptions> options, ILogger<RabbitMqLinkProcessor> logger, IRabbitMqLinkRegister linkRegister, IRabbitMqLinkEndpointFactory linkEndpointFactory)
        {
            _options = options.Value;
            _connectionFactory = new()
            {
                Password = _options.Password,
                UserName = _options.Username,
                HostName = _options.Host,
                Port = _options.Port
            };

            _securityContext = securityContext;
            _logger = logger;
            _linkRegister = linkRegister;
            _linkEndpointFactory = linkEndpointFactory;
        }

        protected RabbitMQ.Client.IConnection Connection
        {
            get
            {
                if (!(_connection?.IsOpen ?? false))
                {
                    lock (_connectionFactory)
                    {
                        if (!(_connection?.IsOpen ?? false))
                        {
                            _connection = _connectionFactory.CreateConnection();
                        }
                    }
                }

                return _connection;
            }
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

            IModel channel = Connection.CreateModel();
            (var linkEndpoint, int initialCredit) = _linkEndpointFactory.CreateEndpoint(channel, attachContext.Attach, attachContext.Link.Role);

            _linkRegister.RegisterLink(((Target)attachContext.Attach.Target).Address, attachContext.Link);

            attachContext.Complete(linkEndpoint, initialCredit);
        }
    }
}
