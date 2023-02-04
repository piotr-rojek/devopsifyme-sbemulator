using ServiceBusEmulator.RabbitMq.Commands;

namespace ServiceBusEmulator.RabbitMq.Endpoints
{
    public interface IRabbitMqManagementCommandFactory
    {
        IManagementCommand GetCommandHandler(string? operation);
    }
}
