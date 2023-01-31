using Amqp.Framing;
using Amqp.Handler;
using System;

namespace ServiceBusEmulator.Azure
{
    internal sealed class AzureHandler : IHandler
    {
        public static AzureHandler Instance { get; } = new AzureHandler();

        private AzureHandler() { }

        public bool CanHandle(EventId id)
        {
            return id is EventId.SendDelivery or EventId.LinkLocalOpen;
        }

        public void Handle(Event protocolEvent)
        {
            if (protocolEvent.Id == EventId.SendDelivery && protocolEvent.Context is IDelivery delivery)
            {
                delivery.Tag = Guid.NewGuid().ToByteArray();
            }

            if (protocolEvent.Id == EventId.LinkLocalOpen && protocolEvent.Context is Attach attach)
            {
                attach.MaxMessageSize = int.MaxValue;
            }
        }
    }
}
