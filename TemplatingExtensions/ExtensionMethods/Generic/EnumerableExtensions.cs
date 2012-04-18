using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplatingExtensions.ExtensionMethods.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Swaps the rows and columns of a nested sequence.
        /// </summary>
        /// <typeparam name="T">The type of elements in the sequence.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <returns>A sequence whose rows and columns are swapped.</returns>
        /// http://higherlogics.blogspot.com/2010/05/linq-transpose-extension-method.html
        public static IEnumerable<IEnumerable<T>> Transpose<T>(
                 this IEnumerable<IEnumerable<T>> source)
        {
            return from row in source
                   from col in row.Select(
                       (x, i) => new KeyValuePair<int, T>(i, x))
                   group col.Value by col.Key into c
                   select c as IEnumerable<T>;
        }

        /// <summary>
        /// Pads jugged 2-dimensional array with a given value to form a rectangular matrix.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Pad<T>(
                this IEnumerable<IEnumerable<T>> source,
                T defaultValue)
        {
            return source.Pad(defaultValue, 0);
        }

        public static IEnumerable<IEnumerable<T>> Pad<T>(
                this IEnumerable<IEnumerable<T>> source,
                T defaultValue,
                int maxCount)
        {
            var minCount = Math.Max(source.Select(row => row.Count()).DefaultIfEmpty().Max(), maxCount);
            return source.Select(row => row.Pad(defaultValue, minCount));
        }

        public static IEnumerable<T> Pad<T>(this IEnumerable<T> source, T defaultValue, int count)
        {
            return source.Concat(Enumerable.Repeat(defaultValue, Math.Max(0, count - source.DefaultIfEmpty().Count())));
        }
    }
}
