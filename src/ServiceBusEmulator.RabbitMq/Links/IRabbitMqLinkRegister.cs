using Amqp.Listener;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public interface IRabbitMqLinkRegister
    {
        ListenerLink? FindByAddress(string address);
        void RegisterLink(string address, ListenerLink link);
    }
}
