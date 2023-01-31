using Amqp.Listener;

namespace ServiceBusEmulator.RabbitMq
{
    public interface IReceiverLinkFinder
    {
        ListenerLink? FindByAddress(string address);
    }
}
