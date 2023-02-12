using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using ServiceBusEmulator.RabbitMq.Options;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public class RabbitMqChannelFactory : IRabbitMqChannelFactory, IDisposable
    {
        private readonly RabbitMqBackendOptions _options;

        private IConnection? _connection;
        private ConnectionFactory _connectionFactory;

        private bool _disposed;

        public RabbitMqChannelFactory(IOptions<RabbitMqBackendOptions> options)
        {
            _options = options.Value;

            _connectionFactory = new()
            {
                Password = _options.Password,
                UserName = _options.Username,
                HostName = _options.Host,
                Port = _options.Port
            };
        }

        protected IConnection Connection
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(RabbitMqChannelFactory));
                }

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

        public IModel CreateChannel()
        {
            return Connection.CreateModel();
        }

        public void Dispose()
        {
            if (!_disposed && _connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
            _disposed = true;
        }
    }
}
