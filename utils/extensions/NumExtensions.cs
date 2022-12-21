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
    }
}
