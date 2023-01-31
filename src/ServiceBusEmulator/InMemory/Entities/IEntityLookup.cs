using System.Collections.Generic;

namespace Xim.Simulators.ServiceBus.InMemory.Entities
{
    public interface IEntityLookup : IEnumerable<(string Address, IEntity Entity)>
    {
        IEntity Find(string name);
    }
}
