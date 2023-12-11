namespace Aoc.Utils.Extensions
{
    public static class CharExtensions
    {
        public static bool IsNumber(this char c)
        {
            return Char.IsNumber(c);
        }

        public static int AsInt(this char c)
        {
            return int.Parse(c.ToString());
        }
    }
}
