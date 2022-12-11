namespace aoc.utils
{
    public readonly struct Vector2D : IEquatable<Vector2D>, IComparable<Vector2D>
    {
        public static readonly Vector2D Zero = new(0, 0);
        public static readonly Vector2D Up = new(0, 1);
        public static readonly Vector2D Down = new(0, -1);
        public static readonly Vector2D Left = new(-1, 0);
        public static readonly Vector2D Right = new(1, 0);

        public int X { get; }
        public int Y { get; }
        public string Id => ToString();

        public Vector2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2D(Vector2D other)
        {
            X = other.X;
            Y = other.Y;
        }

        public bool Equals(Vector2D other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2D other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 353 * X + 359 * Y;
        }

        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        public int CompareTo(Vector2D other)
        {
            return X.CompareTo(other.X) == 0
                ? Y.CompareTo(other.Y)
                : X.CompareTo(other.X);
        }

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return !(v1 == v2);
        }

        public static Vector2D operator *(int k, Vector2D v)
        {
            return new Vector2D(k * v.X, k * v.Y);
        }

        public static Vector2D operator *(Vector2D v, int k)
        {
            return k * v;
        }

        public static Vector2D Normalize(Vector2D v)
        {
            return new Vector2D(Math.Sign(v.X), Math.Sign(v.Y));
        }
    }
}
