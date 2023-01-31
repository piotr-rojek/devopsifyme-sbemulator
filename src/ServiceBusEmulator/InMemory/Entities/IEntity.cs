using Xim.Simulators.ServiceBus.InMemory.Delivering;

namespace Xim.Simulators.ServiceBus.InMemory.Entities
{
    public interface IEntity
    {
        string Name { get; }

        DeliveryQueue DeliveryQueue { get; }

        void Post(Amqp.Message message);
    }
}
