using System;
using Amqp.Framing;
using Amqp.Handler;

namespace Xim.Simulators.ServiceBus.Azure
{
    internal sealed class AzureHandler : IHandler
    {
        public static AzureHandler Instance { get; } = new AzureHandler();

        private AzureHandler() { }

        public bool CanHandle(EventId id)
            => id == EventId.SendDelivery || id == EventId.LinkLocalOpen;

        public void Handle(Event protocolEvent)
        {
            if (protocolEvent.Id == EventId.SendDelivery && protocolEvent.Context is IDelivery delivery)
            {
                delivery.Tag = Guid.NewGuid().ToByteArray();
            }

            if(protocolEvent.Id == EventId.LinkLocalOpen && protocolEvent.Context is Attach attach)
            {
                attach.MaxMessageSize = Int32.MaxValue;
            }
        }
    }
}
