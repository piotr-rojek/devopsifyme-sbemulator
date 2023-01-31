using Amqp.Listener;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBusEmulator.Host;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Xim.Simulators.ServiceBus.InMemory;
using Xim.Simulators.ServiceBus.InMemory.Entities;
using Xim.Simulators.ServiceBus.Options;
using Xim.Simulators.ServiceBus.Security;

namespace Xim.Simulators.ServiceBus
{
    public static class Extensions
    {
        public static IServiceCollection AddServiceBusEmulator(this IServiceCollection services, Action<ServiceBusEmulatorOptions> configure = null)
        {
            configure ??= (o) => { };

            services.AddSingleton<ILinkProcessor, InMemoryLinkProcessor>();
            services.AddSingleton<IEntityLookup, EntityLookup>();

            services.AddTransient<ISecurityContext>(sp => SecurityContext.Default);

            services.AddTransient<CbsRequestProcessor>();
            services.AddTransient<ITokenValidator>(sp => CbsTokenValidator.Default);

            services.AddOptions<ServiceBusEmulatorOptions>().Configure(configure).PostConfigure(options =>
            {
                if(!string.IsNullOrEmpty(options.ServerCertificateThumbprint))
                {
                    using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.ReadOnly);
                    options.ServerCertificate = store.Certificates.Find(X509FindType.FindByThumbprint, options.ServerCertificateThumbprint, false).FirstOrDefault();
                }

                if(!string.IsNullOrEmpty(options.ServerCertificatePath))
                {
                    options.ServerCertificate = new X509Certificate2(options.ServerCertificatePath, options.ServerCertificatePassword, X509KeyStorageFlags.Exportable);
                }
            }).BindConfiguration("Emulator"); ;

            services.AddTransient<ServiceBusEmulatorHost>();
            services.AddSingleton<IHostedService, ServiceBusEmulatorWorker>();

            return services;
        }
    }
}
