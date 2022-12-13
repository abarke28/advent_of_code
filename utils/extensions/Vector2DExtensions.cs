namespace aoc.utils.extensions
{
    public static class Vector2DExtensions
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
    }
}
