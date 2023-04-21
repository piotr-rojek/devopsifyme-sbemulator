using Amqp;
using Amqp.Framing;
using Amqp.Listener;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBusEmulator.Abstractions.Options;
using ServiceBusEmulator.Azure;
using ServiceBusEmulator.Security;
using System;
using System.Net.Security;
using System.Threading.Tasks;

namespace ServiceBusEmulator
{
    public class ServiceBusEmulatorHost
    {
        private bool _disposed;
        private ContainerHost _containerHost;
        private readonly ILinkProcessor _linkProcessor;
        private readonly CbsRequestProcessor _cbsRequestProcessor;
        private readonly ILogger _logger;

        public ServiceBusEmulatorOptions Settings { get; }

        public ServiceBusEmulatorHost(ILinkProcessor linkProcessor, CbsRequestProcessor cbsRequestProcessor, IOptions<ServiceBusEmulatorOptions> options, ILogger<ServiceBusEmulatorHost> logger)
        {
            ServiceBusEmulatorOptions o = options.Value;
            Settings = o;

            _linkProcessor = linkProcessor;
            _cbsRequestProcessor = cbsRequestProcessor;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            try
            {
                if (Settings.ServerCertificate == null)
                {
                    throw new ArgumentNullException(nameof(Settings.ServerCertificate));
                }
                _containerHost = BuildSecureServiceBusHost();
                await StartContainerHostAsync(_containerHost).ConfigureAwait(false);
            }
            catch
            {
                _containerHost?.Close();
                _containerHost = null;
                throw;
            }
        }

        public Task StopAsync()
        {
            return Task.Run(Abort);
        }

        public void Abort()
        {
            try
            {
                StopHost();
            }
            finally
            {
                _containerHost = null;
            }
        }

        private void StopHost()
        {
            _containerHost.Close();
        }

        private Task StartContainerHostAsync(IContainerHost host)
        {
            return Task.Run(() =>
                       {
                           host.RegisterRequestProcessor("$cbs", _cbsRequestProcessor);
                           host.RegisterLinkProcessor(_linkProcessor);
                           host.Open();
                       });
        }

        private ContainerHost BuildSecureServiceBusHost()
        {
            int port = Settings.Port;
            Address address = new($"amqps://localhost:{port}");
            ServiceBusEmulatorContainerHost host = new(new[] { address }, Settings.ServerCertificate);

            host.Listeners[0].HandlerFactory = _ => AzureHandler.Instance;
            host.Listeners[0].SASL.EnableAzureSaslMechanism();
            host.Listeners[0].SASL.EnableExternalMechanism = true;
            host.Listeners[0].SASL.EnableAnonymousMechanism = true;
            host.Listeners[0].SSL.ClientCertificateRequired = true;
            host.Listeners[0].SSL.RemoteCertificateValidationCallback = (_, __, ___, errors) =>
            {
                _logger.LogWarning($"AMQP SSL errors {errors}.");
                return errors == SslPolicyErrors.RemoteCertificateNotAvailable;
            };

            _logger.LogDebug($"Starting secure service bus host at {address}.");

            return host;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            Abort();
        }
    }
}
