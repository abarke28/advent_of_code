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

        public static IEnumerable<Vector2D> Get4Neighbors(this Vector2D source)
        {
            return new List<Vector2D>()
            {
                source + Vector2D.Up,
                source + Vector2D.Down,
                source + Vector2D.Left,
                source + Vector2D.Right
            };
        }

        public static IEnumerable<Vector2D> Get5Neighbors(this Vector2D source)
        {
            return new List<Vector2D>()
            {
                source,
                source + Vector2D.Up,
                source + Vector2D.Down,
                source + Vector2D.Left,
                source + Vector2D.Right
            };
        }

        public static IEnumerable<Vector2D> Get8Neighbors(this Vector2D source)
        {
            foreach (var y in new[] { -1, 0, 1 })
            {
                foreach (var x in new[] { -1, 0, 1 })
                {
                    if (!(y == 0 && x == 0))
                    {
                        yield return new Vector2D(source.X + x, source.Y + y);
                    }
                }
            }
        }

        public static IEnumerable<Vector2D> Get9Neighbors(this Vector2D source)
        {
            foreach (var y in new[] { -1, 0, 1 })
            {
                foreach (var x in new[] { -1, 0, 1 })
                {
                    yield return new Vector2D(source.X + x, source.Y + y);
                }
            }
        }
    }
}
