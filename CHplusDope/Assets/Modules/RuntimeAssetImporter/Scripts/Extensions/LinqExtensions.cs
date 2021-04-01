using System;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeAssetImporter.Extensions
{
    internal static class LinqExtensions
    {
        internal static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }    
        
        internal static T FirstOr<T>(this IEnumerable<T> source, T alternate)
        {
            foreach(T t in source)
                return t;
            return alternate;
        }

        internal static T[] Flatten<T>(this IEnumerable<T> collection, Func<T, T, bool> comparer, Func<T, T, T> combiner)
        {
            var temp = collection.ToArray();
            var result = new List<T>(temp.Length);
            foreach (var entry in temp)
            {
                var existingIndex = -1;
                for (var i = 0; i < result.Count; i++)
                {
                    var existing = result[i];
                    if (!comparer(entry, existing)) continue;
                    existingIndex = i;
                    break;
                }

                if (existingIndex < 0)
                {
                    result.Add(entry);
                    continue;
                }

                var combined = combiner(entry, result[existingIndex]);
                result[existingIndex] = combined;
            }
            return result.ToArray();
        }

        internal static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var entry in collection)
            {
                if (entry == null) continue;
                action(entry);
            }
        }
    }
}