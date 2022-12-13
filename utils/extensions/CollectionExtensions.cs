using System.Collections;

namespace aoc.utils.extensions
{
    public static class CollectionExtensions
    {
        public static int ReadAsBinary(this IEnumerable<int> bits)
        {
            var bitArray = new BitArray(bits.Select(b => b == 1).ToArray());

            return bitArray.ConvertToInt();
        }

        public static IDictionary<int, T> ToIndexedDictionary<T>(this IEnumerable<T> source, int indexOffset = 0)
        {
            var tupleList = source.Select<T, (T Item, int Index)>((x, n) => new (x, n + indexOffset));

            var dict = tupleList.ToDictionary(i => i.Index, i => i.Item);

            return dict;
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
    }
}
