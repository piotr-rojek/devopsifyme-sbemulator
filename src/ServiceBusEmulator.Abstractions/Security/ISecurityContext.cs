using Amqp;

namespace Xim.Simulators.ServiceBus
{
    public interface ISecurityContext
    {
        void Authorize(Connection connection);

        bool IsAuthorized(Connection connection);
    }
}
