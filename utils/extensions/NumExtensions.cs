namespace aoc.utils.extensions
{
    public static class NumExtensions
    {
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
