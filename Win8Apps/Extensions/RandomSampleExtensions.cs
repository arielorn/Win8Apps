using System;
using System.Collections.Generic;
using System.Linq;

namespace Win8Apps.Extensions
{
    public static class RandomSampleExtensions
    {
        private static readonly Random Random = new Random();

        public static T RandomSingleSample<T>(this IEnumerable<T> source, int seed = -1)
        {
            List<T> buffer;
            if (source is List<T>)
                buffer = (List<T>)source;
            else
                buffer = new List<T>(source);

            if (buffer.Count == 0)
                return default(T);

            Random random;
            if (seed < 0)
                random = Random;
            else
                random = new Random(seed);

            int randomIndex = random.Next(buffer.Count);
            return buffer[randomIndex];
        }

        public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> source, int count, bool allowDuplicates = false, int seed = -1)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return RandomSampleIterator(source, count, seed, allowDuplicates);
        }

        private static IEnumerable<T> RandomSampleIterator<T>(IEnumerable<T> source, int count, int seed, bool allowDuplicates)
        {
            // take a copy of the current list
            List<T> buffer;
            if (source is List<T>)
                buffer = (List<T>)source;
            else
                buffer = new List<T>(source);

            // create the "random" generator, time dependent or with the specified seed
            Random random;
            if (seed < 0)
                random = Random;
            else
                random = new Random(seed);

            count = Math.Min(count, buffer.Count);

            var selected = new List<int>(count);
            if (count > 0)
            {
                while (selected.Count < count &&
                       selected.Count < buffer.Count)
                {
                    int randomIndex = random.Next(buffer.Count);
                    if (allowDuplicates || !selected.Contains(randomIndex))
                    {
                        yield return buffer[randomIndex];
                        selected.Add(randomIndex);
                    }
                }
            }
            //if (count > 0)
            //{
            //    // iterate count times and "randomly" return one of the 
            //    // elements
            //    for (int i = 1; i <= count; i++)
            //    {
            //        // maximum index actually buffer.Count -1 because 
            //        // Random.Next will only return values LESS than 
            //        // specified.
            //        int randomIndex = random.Next(buffer.Count);
            //        yield return buffer[randomIndex];
            //        if (!allowDuplicates)
            //            // remove the element so it can't be selected a 
            //            // second time
            //            buffer.RemoveAt(randomIndex);
            //    }
            //}
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source, int seed = -1)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Random random;
            if (seed < 0)
                random = Random;
            else
                random = new Random(seed);

            return source.OrderBy(i => random.Next());
        }

        public static List<T> Randomize<T>(this List<T> source, int seed = -1)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Random random;
            if (seed < 0)
                random = Random;
            else
                random = new Random(seed);

            return source.OrderBy(i => random.Next()).ToList();
        }
    }
}