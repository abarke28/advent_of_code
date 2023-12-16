namespace Aoc.Utils
{
    public class Pose
    {
        public Vector2D Pos { get; set; }
        public Vector2D Ahead { get; set; }

        public Pose(Vector2D pos, Vector2D ahead)
        {
            Pos = pos;
            Ahead = ahead;
        }

        public string GetRepresentativeString()
        {
            return $"{Pos}-{Ahead}";
        }
    }
}
