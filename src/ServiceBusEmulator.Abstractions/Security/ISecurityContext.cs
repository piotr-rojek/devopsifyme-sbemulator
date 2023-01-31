using Amqp;

namespace ServiceBusEmulator.Abstractions.Security
{
    public interface ISecurityContext
    {
        void Authorize(Connection connection);

        bool IsAuthorized(Connection connection);
    }
}
