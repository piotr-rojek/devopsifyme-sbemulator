using Amqp;
using Amqp.Listener;
using System.Collections.Concurrent;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public abstract class RabbitMqLinkRegister: IRabbitMqLinkRegister
    {
        private ConcurrentDictionary<string, ListenerLink> OutgoingLinks { get; } = new ConcurrentDictionary<string, ListenerLink>();

        public void RegisterLink(string address, ListenerLink link)
        {
            _ = OutgoingLinks.TryAdd(address, link);
            link.Closed += (s, e) => OutgoingLinks.TryRemove(address, out _);
        }

        public ListenerLink? FindByAddress(string address)
        {
            return OutgoingLinks.TryGetValue(address, out ListenerLink? link) ? link : null;
        }
    }
}
