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

        public Vector2D Transpose()
        {
            return new Vector2D(Y, X);
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

        public IEnumerable<Vector2D> Get4Neighbors()
        {
            return new List<Vector2D>()
            {
                this + Vector2D.Up,
                this + Vector2D.Down,
                this + Vector2D.Left,
                this + Vector2D.Right
            };
        }

        public IEnumerable<Vector2D> Get5Neighbors()
        {
            return new List<Vector2D>()
            {
                this,
                this + Vector2D.Up,
                this + Vector2D.Down,
                this + Vector2D.Left,
                this + Vector2D.Right
            };
        }

        public IEnumerable<Vector2D> Get8Neighbors()
        {
            foreach (var y in new[] { -1, 0, 1 })
            {
                foreach (var x in new[] { -1, 0, 1 })
                {
                    if (!(y == 0 && x == 0))
                    {
                        yield return new Vector2D(X + x, Y + y);
                    }
                }
            }
        }

        public IEnumerable<Vector2D> Get9Neighbors()
        {
            foreach (var y in new[] { -1, 0, 1 })
            {
                foreach (var x in new[] { -1, 0, 1 })
                {
                    yield return new Vector2D(X + x, Y + y);
                }
            }
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
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

        public static IList<Vector2D> GetPointsBetween(Vector2D v1, Vector2D v2)
        {
            var delta = v2 - v1;

            if (delta.X != 0 && delta.Y != 0 && Math.Abs(delta.X) != Math.Abs(delta.Y))
            {
                throw new Exception("Vectors not rectolinear or diagonal");
            }

            var increment = Normalize(delta);
            var current = v1 + increment;

            var points = new List<Vector2D>(Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y)) + 1)
            {
                current
            };

            while (current != v2)
            {
                current += increment;
                points.Add(current);
            }

            return points;
        }

        public static int ManhattanDistance(Vector2D v1, Vector2D v2)
        {
            return Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y);
        }
    }
}
