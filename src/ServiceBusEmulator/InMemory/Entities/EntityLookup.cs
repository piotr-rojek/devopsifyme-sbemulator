using Microsoft.Extensions.Options;
using ServiceBusEmulator.Abstractions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ServiceBusEmulator.InMemory.Entities
{
    internal class EntityLookup : IEntityLookup
    {
        private readonly Dictionary<string, IEntity> _entities;

        public EntityLookup(IOptions<ServiceBusEmulatorOptions> options)
        {
            ServiceBusEmulatorOptions o = options.Value;

            var topics = o.Topics
                .Select(topic => new
                {
                    topic.Name,
                    Entity = (IEntity)topic
                });

            var subscriptions = o.Topics
                .SelectMany(topic => topic
                    .Subscriptions
                    .Select(subscription => new
                    {
                        Name = $"{topic.Name}/Subscriptions/{subscription.Name}",
                        Entity = (IEntity)subscription
                    })
                );

            var queues = o.Queues
                .Select(queue => new
                {
                    Name = $"/{queue.Name}",
                    Entity = (IEntity)new QueueEntity(queue.Name)
                });

            _entities = topics
                .Concat(subscriptions)
                .Concat(queues)
                .ToDictionary(
                    item => item.Name,
                    item => item.Entity,
                    StringComparer.OrdinalIgnoreCase
                );
        }

        public IEntity Find(string name)
        {
            return _entities.TryGetValue(name, out IEntity entity)
                        ? entity
                        : null;
        }

        public IEnumerator<(string Address, IEntity Entity)> GetEnumerator()
        {
            foreach (KeyValuePair<string, IEntity> item in _entities)
            {
                yield return (item.Key, item.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
