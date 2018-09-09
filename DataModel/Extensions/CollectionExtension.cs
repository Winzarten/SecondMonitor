namespace SecondMonitor.DataModel.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class CollectionExtension
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T x in collection)
            {
                action(x);
            }
        }
    }
}