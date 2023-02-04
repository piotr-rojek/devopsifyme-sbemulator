using Microsoft.Extensions.DependencyInjection;
using ServiceBusEmulator.Abstractions.Azure;
using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-amqp-request-response
    /// </summary>
    public class RabbitMqManagementCommandFactory : IRabbitMqManagementCommandFactory
    {
        private readonly IServiceProvider _services;

        public RabbitMqManagementCommandFactory(IServiceProvider services)
        {
            _services = services;
        }

        public IManagementCommand GetCommandHandler(string? operation)
        {
            return operation switch
            {
                ManagementConstants.Operations.RenewLockOperation => _services.GetRequiredService<RenewLockCommand>(),
                ManagementConstants.Operations.PeekMessageOperation => _services.GetRequiredService<PeekMessageCommand>(),
                ManagementConstants.Operations.RenewSessionLockOperation => _services.GetRequiredService<RenewSessionLockCommand>(),
                ManagementConstants.Operations.SetSessionStateOperation => _services.GetRequiredService<SetSessionStateCommand>(),
                ManagementConstants.Operations.GetSessionStateOperation => _services.GetRequiredService<GetSessionStateCommand>(),
                _ => throw new NotImplementedException($"Operation {operation} not implemented")
            };

        }
    }
}
