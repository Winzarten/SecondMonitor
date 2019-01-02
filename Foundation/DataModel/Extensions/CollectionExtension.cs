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

        public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, TSource, TResult> projection)
        {
            using (IEnumerator<TSource> iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    yield break;
                }
                TSource previous = iterator.Current;
                while (iterator.MoveNext())
                {
                    yield return projection(previous, iterator.Current);
                    previous = iterator.Current;
                }
            }
        }

        public static IEnumerable<TSource> WhereWithPrevious<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> checkFunction)
        {
            using (IEnumerator<TSource> iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                {
                    yield break;
                }
                TSource previous = iterator.Current;
                yield return previous;

                while (iterator.MoveNext())
                {
                    if (!checkFunction(previous, iterator.Current))
                    {
                        continue;
                    }

                    previous = iterator.Current;
                    yield return iterator.Current;
                }
            }
        }
    }
}