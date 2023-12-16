namespace Aoc.Utils
{
    public class Pose
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
    }
}
