using System.Numerics;

namespace Aoc.Utils.Extensions
{
    public static class NumExtensions
    {
        public static bool IsEven<T> (this T value) where T : INumber<T>
        {
            return value % (T.One + T.One) == T.Zero;
        }

        public static bool IsOdd<T>(this T value) where T : INumber<T>
        {
            return !IsEven(value);
        }

        public static int MathMod(this int a, int b)
        {
            return ((a % b) + b) % b;
        }

        public static long MathMod(this long a, long b)
        {
            return ((a % b) + b) % b;
        }

        public static int ModWithOffset(this int a, int b, int offset)
        {
            return ((a - offset) % b) + offset;
        }

        public static int ModWithOffset(this long a, int b, int offset)
        {
            return (int)(((a - offset) % b) + offset);
        }
    }
}
