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
    }
}
