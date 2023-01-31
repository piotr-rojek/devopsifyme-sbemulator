using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xim.Simulators.ServiceBus
{
    internal static class CollectionExtensions
    {
        internal static IReadOnlyList<T> AsReadOnly<T>(this T[] array)
            => Array.AsReadOnly(array);

        internal static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
            => new ReadOnlyDictionary<TKey, TValue>(dictionary);
    }
}
