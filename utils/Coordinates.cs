namespace Aoc.Utils
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsTouching(Coordinate other)
        {
            var otherX = other.X;
            var otherY = other.Y;

            if ((Math.Abs(otherX - X) <= 1) && (Math.Abs(otherY - Y) <= 1))
            {
                return true;
            }

            return false;
        }

        public bool IsOnSameRow(Coordinate other)
        {
            return (Y == other.Y);
        }

        public bool IsOnSameColumn(Coordinate other)
        {
            return (X == other.X);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
