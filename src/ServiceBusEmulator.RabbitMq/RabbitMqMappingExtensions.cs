using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xim.Simulators.ServiceBus.Rabbit
{
    public static class RabbitMqMappingExtensions
    {
        public static T GetHeader<T>(this IBasicProperties prop, string key)
        {
            if (prop.Headers == null || !prop.Headers.ContainsKey(key))
            {
                return default(T);
            }

            var value = prop.Headers[key];
            if (value is byte[])
            {
                value = System.Text.Encoding.UTF8.GetString((byte[])value);
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static IEnumerable<(string key, T value)> GetHeadersStartingWith<T>(this IBasicProperties prop, string keyPrefix)
        {
            if (prop.Headers == null)
            {
                yield break;
            }

            foreach (var x in prop.Headers.Where(it => it.Key.StartsWith(keyPrefix)))
            {
                var key = x.Key.Substring(keyPrefix.Length);
                yield return (key, prop.GetHeader<T>(x.Key));
            }
        }
    }
}
