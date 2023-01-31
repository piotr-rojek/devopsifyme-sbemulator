using Amqp;
using Amqp.Listener;
using System.Threading;

namespace ServiceBusEmulator.InMemory.Delivering
{
    internal interface IDeliveryQueue
    {
        void Enqueue(Delivery delivery);

        Message Dequeue(CancellationToken cancellationToken);

        void Process(MessageContext messageContext);
    }
}
