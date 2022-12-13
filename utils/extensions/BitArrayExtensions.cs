using System.Collections;

namespace aoc.utils.extensions
{
    public static class BitArrayExtensions
    {
        public static int ConvertToInt(this BitArray source)
        {
            if (source.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            source.Reverse().CopyTo(array, 0);
            return array[0];
        }

        public static BitArray Reverse(this BitArray source)
        {
            int length = source.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                (source[length - i - 1], source[i]) = (source[i], source[length - i - 1]);
            }

            return source;
        }

        public static BitArray Duplicate(this BitArray source)
        {
            return (source.Clone() as BitArray)!;
        }
    }
}
