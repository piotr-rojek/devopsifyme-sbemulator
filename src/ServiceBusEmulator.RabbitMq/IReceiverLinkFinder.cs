using Amqp.Listener;

namespace Xim.Simulators.ServiceBus.InMemory
{
    public interface IReceiverLinkFinder
    {
        ListenerLink FindByAddress(string address);
    }
}
