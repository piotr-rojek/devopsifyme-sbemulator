using Amqp.Listener;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusEmulator.RabbitMq.Commands;
using ServiceBusEmulator.RabbitMq.Endpoints;
using ServiceBusEmulator.RabbitMq.Options;

namespace ServiceBusEmulator.RabbitMq
{
    public static class Extensions
    {
        public static IServiceCollection AddServiceBusEmulatorRabbitMqBackend(this IServiceCollection services, Action<RabbitMqBackendOptions>? configure = null)
        {
            configure ??= (o) => { };

            _ = services.AddSingleton<ILinkProcessor, RabbitMqLinkProcessor>();
            _ = services.AddSingleton(sp => (IReceiverLinkFinder)sp.GetRequiredService<ILinkProcessor>());

            _ = services.AddTransient<IRabbitMqUtilities, RabbitMqUtilities>();
            _ = services.AddTransient<IRabbitMqMapper, RabbitMqMapper>();
            _ = services.AddTransient<RabbitMqSenderEndpoint>();
            _ = services.AddTransient<RabbitMqReceiverEndpoint>();
            _ = services.AddTransient<RabbitMqManagementSenderEndpoint>();
            _ = services.AddTransient<RabbitMqManagementReceiverEndpoint>();

            _ = services.AddTransient<RenewLockCommand>();
            _ = services.AddTransient<PeekMessageCommand>();
            _ = services.AddTransient<RenewSessionLockCommand>();
            _ = services.AddTransient<SetSessionStateCommand>();
            _ = services.AddTransient<GetSessionStateCommand>();

            _ = services.AddOptions<RabbitMqBackendOptions>().Configure(configure).BindConfiguration("Emulator:RabbitMq");

            return services;
        }
    }
}
