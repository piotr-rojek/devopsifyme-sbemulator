using ServiceBusEmulator.InMemory.Delivering;

namespace ServiceBusEmulator.InMemory.Entities
{
    public interface IEntity
    {
        string Name { get; }

        DeliveryQueue DeliveryQueue { get; }

        void Post(Amqp.Message message);
    }
}
