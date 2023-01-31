using System;
using System.Net.Security;
using System.Threading.Tasks;
using Amqp;
using Amqp.Listener;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xim.Simulators.ServiceBus.Azure;
using Xim.Simulators.ServiceBus.Options;
using Xim.Simulators.ServiceBus.Security;

namespace Xim.Simulators.ServiceBus
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
            var o = options.Value;
            Settings = o;

            _linkProcessor = linkProcessor;
            _cbsRequestProcessor = cbsRequestProcessor;
            _logger = logger;
        }

        public async Task StartAsync()
        {
            try
            {
                if(Settings.ServerCertificate == null)
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
            => Task.Run(Abort);

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
            => Task.Run(() =>
               {
                   host.RegisterRequestProcessor("$cbs", _cbsRequestProcessor);
                   host.RegisterLinkProcessor(_linkProcessor);
                   host.Open();
               });


        private ContainerHost BuildSecureServiceBusHost()
        {
            var port = Settings.Port;
            var address = new Address($"amqps://localhost:{port}");
            var host = new ContainerHost(new[] { address }, Settings.ServerCertificate);

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
