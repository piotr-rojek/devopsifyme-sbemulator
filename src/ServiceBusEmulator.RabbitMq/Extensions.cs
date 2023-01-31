using Xim.Simulators.ServiceBus.InMemory;
using Xim.Simulators.ServiceBus.Rabbit.Endpoints;
using Xim.Simulators.ServiceBus.Rabbit.Management;
using Xim.Simulators.ServiceBus.Rabbit;
using Microsoft.Extensions.DependencyInjection;
using Amqp.Listener;
using Xim.Simulators.ServiceBus.Options;

namespace ServiceBusEmulator.RabbitMq
{
    public static class Extensions
    {
        public static IServiceCollection AddServiceBusEmulatorRabbitMqBackend(this IServiceCollection services, Action<RabbitMqBackendOptions> configure = null)
        {
            configure ??= (o) => { };

            services.AddSingleton<ILinkProcessor, RabbitMqLinkProcessor>();
            services.AddSingleton(sp => sp.GetRequiredService<ILinkProcessor>() as IReceiverLinkFinder);

            services.AddTransient<RabbitMqUtilities>();
            services.AddTransient<RabbitMqMapper>();
            services.AddTransient<RabbitMqSenderEndpoint>();
            services.AddTransient<RabbitMqReceiverEndpoint>();
            services.AddTransient<RabbitMqManagementSenderEndpoint>();
            services.AddTransient<RabbitMqManagementReceiverEndpoint>();

            services.AddTransient<RenewLockCommand>();
            services.AddTransient<PeekMessageCommand>();
            services.AddTransient<RenewSessionLockCommand>();
            services.AddTransient<SetSessionStateCommand>();
            services.AddTransient<GetSessionStateCommand>();

            services.AddOptions<RabbitMqBackendOptions>().Configure(configure).BindConfiguration("Emulator:RabbitMq");

            return services;
        }
    }
}
