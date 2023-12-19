using System.Collections;
using System.Numerics;

namespace Aoc.Utils.Extensions
{
    public static class CollectionExtensions
    {
        public static int ReadAsBinary(this IEnumerable<int> bits)
        {
            var bitArray = new BitArray(bits.Select(b => b == 1).ToArray());

            return bitArray.ConvertToInt();
        }

        public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> source, int indexOffset = 0)
        {
            return source.Select<T, (T Item, int Index)>((item, index) => new(item, index + indexOffset));
        }

        public static IDictionary<int, T> ToIndexedDictionary<T>(this IEnumerable<T> source, int indexOffset = 0)
        {
            var dict = source.WithIndex(indexOffset).ToDictionary(i => i.Index, i => i.Item);

            return dict;
        }

        public static Dictionary<TValue, TKey> Invert<TKey, TValue>(this IDictionary<TKey, TValue> source) where TValue : notnull
        {
            return source.ToDictionary(source => source.Value, source => source.Key);
        }

        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                source.Add(item);
            }
        }

        public static IEnumerable<(T1? First, T2? Second)> ZipAll<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
        {
            var firstCount = first.Count();
            var secondCount = second.Count();

            var castedFirst = first.Select(i => (T1?)i);
            var castedSecond = second.Select(i => (T2?)i);

            if (firstCount == secondCount)
            {
                return castedFirst.Zip(castedSecond);
            }

            var delta = Math.Abs(firstCount - secondCount);

            if (firstCount > secondCount)
            {
                var expandedSecond = new List<T2?>(firstCount);
                expandedSecond.AddRange(second);
                expandedSecond.AddRange(Enumerable.Repeat(default(T2), delta));

                return castedFirst.Zip(expandedSecond);
            }
            else
            {
                var expandedFirst = new List<T1?>(secondCount);
                expandedFirst.AddRange(first);
                expandedFirst.AddRange(Enumerable.Repeat(default(T1), delta));

                return expandedFirst.Zip(castedSecond);
            }
        }

        /// <summary>
        /// Chunks the source IEnumerable into chunks based off of a predicate, allowing dynamic & non-uniform chunk size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="chunkPredicate">Predicate to determine which records to take in each chunk.</param>
        /// <param name="skipPredicate">Predicate to determine which records to skip between each chunk. If not supplied, will use the negation of the <see cref="chunkPredicate">chunkPredicate</see>.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, Func<T, bool> chunkPredicate, Func<T, bool>? skipPredicate = null)
        {
            var chunks = new List<List<T>>();

            var i = 0;
            var length = source.Count();

            while (i < length)
            {
                var chunk = source
                    .Skip(i)
                    .TakeWhile(r => chunkPredicate.Invoke(r))
                    .ToList();

                chunks.Add(chunk);
                i += chunk.Count;

                var skipCount = source
                    .Skip(i)
                    .TakeWhile(r => skipPredicate == null ? !chunkPredicate.Invoke(r) : skipPredicate.Invoke(r))
                    .Count();

                i += skipCount;
            }

            return chunks;
        }

        public static T Product<T>(this IEnumerable<T> nums) where T : INumber<T>
        {
            return nums.Aggregate(T.One, (n1, n2) => n1 * n2);
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> source) where T : INumber<T>
        {
            return MathUtils.CartesianProduct(source);
        }

        public static long CalculatePolygonArea(this IEnumerable<Vector2D> vertices)
        {
            // Shoelace Formula:
            // A = 1/2 |(v0 ^ v1 + v1 ^ v2 + ... vn-1 ^ v0)|
            // Where A ^ B = A.x * B.y - B.x * A.y
            var shoeLaceSum = 0L;
            var loop = vertices.ToList();

            for (int i = 0; i < loop.Count; i++)
            {
                var v1 = loop[i];
                var v2 = loop[(i + 1) % loop.Count];

                var wedge = (long)v1.X * (long)v2.Y - (long)v2.X * (long)v1.Y;

                shoeLaceSum += wedge;
            }

            shoeLaceSum /= 2;

            return Math.Abs(shoeLaceSum);
        }
    }
}
