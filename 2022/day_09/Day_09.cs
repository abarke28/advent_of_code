using aoc.common;
using aoc.utils;

namespace aoc.y2022.day_09
{
    // https://adventofcode.com/2022/day/09
    public class Day_09 : ISolver
    {
        private static readonly Dictionary<char, Vector2D> InstructionMoveMap = new Dictionary<char, Vector2D>()
        {
            { 'R', Vector2D.Right },
            { 'L', Vector2D.Left },
            { 'U', Vector2D.Up },
            { 'D', Vector2D.Down }
        };

        private class RopeInstruction
        {
            public Vector2D Direction { get; set; }
            public int Count { get; set; }

            public static RopeInstruction FromString(string s)
            {
                var words = s.Split(' ');

                return new RopeInstruction
                {
                    Direction = InstructionMoveMap[words[0].Single()],
                    Count = int.Parse(words[1])
                };
            }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_09/input.txt");

            var instructions = lines.Select(l => RopeInstruction.FromString(l));

            var moves = instructions.SelectMany(i => ParseInstructionToMoves(i));

            var len2Locations = ComputeTailLocationsFromMoves(moves, 2);

            Console.WriteLine(len2Locations.Count);

            var len10Locations = ComputeTailLocationsFromMoves(moves, 10);

            Console.WriteLine(len10Locations.Count);
        }

        private static HashSet<Vector2D> ComputeTailLocationsFromMoves(IEnumerable<Vector2D> moves, int numSegments)
        {
            var segments = Enumerable.Repeat(Vector2D.Zero, numSegments).ToList();

            var tailVisits = new HashSet<Vector2D>
            {
                segments[^1]
            };

            foreach (var move in moves)
            {
                segments[0] += move;

                for (var i = 1; i < segments.Count; i++)
                {
                    segments[i] = ComputeFollowerLocation(segments[i], segments[i-1]);
                }

                tailVisits.Add(segments[^1]);
            }

            return tailVisits;
        }

        private static Vector2D ComputeFollowerLocation(Vector2D follower, Vector2D leader)
        {
            if (follower.IsAdjacent(leader))
            {
                return follower;
            }

            var delta = leader - follower;
            var move = new Vector2D(Math.Clamp(delta.X, -1, 1), Math.Clamp(delta.Y, -1, 1));

            return follower + move;
        }

        private static IEnumerable<Vector2D> ParseInstructionToMoves(RopeInstruction instruction)
        {
            var moves = new List<Vector2D>();

            while (instruction.Count-- > 0)
            {
                moves.Add(instruction.Direction);
            }

            return moves;
        }
    }
}
