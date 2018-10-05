namespace SecondMonitor.DataModel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtension
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T x in collection)
            {
                action(x);
            }
        }

        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, bool> predicate)
        {
            IEnumerable<TKey> toRemove = dictionary.Keys.Where(predicate);
            toRemove.ForEach(x => dictionary.Remove(x));
        }
    }
}