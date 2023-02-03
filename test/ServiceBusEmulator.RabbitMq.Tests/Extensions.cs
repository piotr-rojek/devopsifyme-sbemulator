using Amqp.Types;
using AutoFixture;

namespace ServiceBusEmulator.RabbitMq.Tests
{
    public static class Extensions
    {
        public static Map ToMap<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            Map map = new();

            foreach (var item in dictionary)
            {
                map[item.Key] = item.Value;
            }

            return map;
        }

        public static TMap ToDescribedMap<TMap, TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        where TMap : DescribedMap, new()
        {
            TMap map = new();

            foreach (var item in dictionary)
            {
                map[item.Key] = item.Value;
            }

            return map;
        }

        public static TMap CreateDescribedMap<TMap, TKey, TValue>(this IFixture fixture)
            where TMap : DescribedMap, new()
        {
            IDictionary<TKey, TValue> dictionary = fixture.Create<Dictionary<TKey, TValue>>();
            return dictionary.ToDescribedMap<TMap, TKey, TValue>();
        }

        public static IDictionary<object, string> FromDescribedMap(this DescribedMap map)
        {
            Dictionary<object, string> dictionary = new();

            foreach (var item in map.Map)
            {
                map[item.Key] = item.Value;
            }

            return dictionary;
        }
    }
}
