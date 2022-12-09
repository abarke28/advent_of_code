namespace aoc.utils
{
    public readonly struct Vector3D
    {
        public static readonly Vector3D Zero = new(0, 0, 0);
        public static readonly Vector3D XHat = new(1, 0, 0);
        public static readonly Vector3D YHat = new(0, 1, 0);
        public static readonly Vector3D ZHat = new(0, 0, 1);

        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        public string Id => ToString();

        public Vector3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D(Vector3D other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public bool Equals(Vector3D other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector3D other && Equals(other);
        }

        public override int GetHashCode()
        {
            return 353 * X + 359 * Y + 367 * Z;
        }

        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return !(v1 == v2);
        }

        public static Vector3D operator *(Vector3D v1, Vector3D v2)
        {
            var x = v1.Y * v2.Z - v1.Z * v2.Y;
            var y = v1.Z * v2.X - v1.X * v2.Z;
            var z = v1.X * v2.Y - v1.Y * v2.X;

            return new Vector3D(x, y, z);
        }

        public static Vector3D operator *(int k, Vector3D v)
        {
            return new Vector3D(k * v.X, k * v.Y, k * v.Z);
        }

        public static Vector3D operator *(Vector3D v, int k)
        {
            return k * v;
        }
    }
}
