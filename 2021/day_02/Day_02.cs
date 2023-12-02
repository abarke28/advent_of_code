using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2021.day_02
{
    // https://adventofcode.com/2021/day/02
    public class Day_02
    {
        private static readonly Dictionary<string, Vector2D> DirectionVectorMap2D = new()
        {
            { "forward", Vector2D.Right },
            { "up", Vector2D.Down },
            { "down", Vector2D.Up }
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_02/input.txt");

            var directions = lines.Select(l => ParseInstruction(l));

            var position = directions.Sum();

            Console.WriteLine(position.Y * position.X);

            var positionv2 = Vector3D.Zero;

            foreach (var l in lines)
            {
                positionv2 = ExecuteDirection(l, positionv2);
            }

            Console.WriteLine(positionv2.X * positionv2.Y);
        }

        private static Vector2D ParseInstruction(string s)
        {
            var direction = s.GetWords().First();
            var distance = s.ReadAllNumbers().Single();

            return distance * DirectionVectorMap2D[direction];
        }

        private static Vector3D ExecuteDirection(string s, Vector3D currentPosition)
        {
            var direction = s.GetWords().First();
            var distance = s.ReadAllNumbers().Single();

            return direction switch
            {
                "up" => currentPosition - (distance * Vector3D.ZHat),
                "down" => currentPosition + (distance * Vector3D.ZHat),
                "forward" => currentPosition + (distance * Vector3D.XHat) + (distance * currentPosition.Z * Vector3D.YHat),
                _ => throw new Exception("Unknown direction"),
            };
        }
    }
}
