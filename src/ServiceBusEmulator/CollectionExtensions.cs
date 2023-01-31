using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ServiceBusEmulator
{
    internal static class CollectionExtensions
    {
        internal static IReadOnlyList<T> AsReadOnly<T>(this T[] array)
        {
            return Array.AsReadOnly(array);
        }

        internal static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}
