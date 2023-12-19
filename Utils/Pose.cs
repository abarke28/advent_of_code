namespace Aoc.Utils
{
    public struct Pose : IEquatable<Pose>
    {
        public Vector2D Pos { get; set; }
        public Vector2D Face { get; set; }

        public Vector2D Ahead => Pos + Face;
        public Vector2D Behind => Pos - Face;

        public Pose(Vector2D pos, Vector2D face)
        {
            Pos = pos;
            Face = face;
        }

        public string GetRepresentativeString()
        {
            return $"Pos={Pos}, Face={Face}";
        }

        public bool Equals(Pose other)
        {
            return this.GetRepresentativeString().Equals(other.GetRepresentativeString());
        }

        public override int GetHashCode()
        {
            return GetRepresentativeString().GetHashCode();
        }
    }
}
