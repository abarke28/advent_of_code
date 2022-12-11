using System.Collections;

namespace aoc.utils
{
    public static class Extensions
    {
        public static bool IsAdjacent(this Vector2D v, Vector2D other)
        {
            return Math.Abs(v.X - other.X) <= 1 && Math.Abs(v.Y - other.Y) <= 1;
        }

        public static bool IsCardinal(this Vector2D v, Vector2D other)
        {
            return IsOnSameRow(v, other) != IsOnSameColumn(v, other);
        }

        public static bool IsDiagonal(this Vector2D v, Vector2D other)
        {
            return !v.IsCardinal(other);
        }

        public static bool IsCoincident(this Vector2D v, Vector2D other)
        {
            return v.Equals(other);
        }

        public static bool IsOnSameRow(this Vector2D v, Vector2D other)
        {
            return v.Y == other.Y;
        }

        public static bool IsOnSameColumn(this Vector2D v, Vector2D other)
        {
            return v.X == other.X;
        }

        public static Vector2D Sum(this IEnumerable<Vector2D> source)
        {
            var sum = Vector2D.Zero;

            foreach (var v in source)
            {
                sum += v;
            }

            return sum;
        }

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
                bool bit = source[i];
                source[i] = source[length - i - 1];
                source[length - i - 1] = bit;
            }

            return source;
        }

        public static BitArray Duplicate(this BitArray source)
        {
            return (source.Clone() as BitArray)!;
        }

        public static int ReadAsBinary(this IEnumerable<int> bits)
        {
            var bitArray = new BitArray(bits.Select(b => b == 1).ToArray());

            return bitArray.ConvertToInt();
        }
    }
}
