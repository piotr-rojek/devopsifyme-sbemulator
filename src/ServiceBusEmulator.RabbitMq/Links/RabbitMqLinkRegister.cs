using Amqp;
using Amqp.Listener;
using System.Collections.Concurrent;

namespace ServiceBusEmulator.RabbitMq.Links
{
    public class RabbitMqLinkRegister: IRabbitMqLinkRegister
    {
        private ConcurrentDictionary<string, ListenerLink> OutgoingLinks { get; } = new ConcurrentDictionary<string, ListenerLink>();

        public void RegisterLink(string address, ListenerLink link)
        {
            if(string.IsNullOrEmpty(address))
            {
                return;
            }

            _ = OutgoingLinks.TryAdd(address, link);
            link.Closed += (s, e) => OutgoingLinks.TryRemove(address, out _);
        }

        public ListenerLink? FindByAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            return OutgoingLinks.TryGetValue(address, out ListenerLink? link) ? link : null;
        }
    }
}
