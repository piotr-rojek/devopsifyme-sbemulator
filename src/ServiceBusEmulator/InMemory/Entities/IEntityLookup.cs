using System.Collections.Generic;

namespace ServiceBusEmulator.InMemory.Entities
{
    public interface IEntityLookup : IEnumerable<(string Address, IEntity Entity)>
    {
        IEntity Find(string name);
    }
}
