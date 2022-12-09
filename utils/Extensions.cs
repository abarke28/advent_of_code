using aoc.utils;

namespace aoc.utils
{
    public static class Extensions
    {
        public static bool IsAdjacent(this Vector2D v, Vector2D other)
        {
            return Math.Abs(v.X - other.X) <= 1 && Math.Abs(v.Y - other.Y) <= 1;
        }

        public static bool IsOnSameRow(this Vector2D v, Vector2D other)
        {
            return v.Y == other.Y;
        }

        public static bool IsOnSameColumn(this Vector2D v, Vector2D other)
        {
            return v.X == other.X;
        }
    }
}
