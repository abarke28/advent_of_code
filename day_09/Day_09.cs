using aoc.common;
using aoc.utils;

namespace aoc.day_09
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
            var lines = FileUtils.ReadAllLines("day_09/input.txt");

            var instructions = lines.Select(l => RopeInstruction.FromString(l));

            var moves = instructions.SelectMany(i => ParseInstructionToMoves(i));

            var tailLocations = ComputeTailLocationsFromMoves(moves);

            Console.WriteLine(tailLocations.Select(tl => tl.ToString()).ToHashSet().Count);

            var finalLocations = ComputeFollowerLocationsForNKnots(tailLocations, 9);

            Console.WriteLine(finalLocations.Select(l => l.ToString()).ToHashSet().Count);
        }

        private static List<Vector2D> ComputeFollowerLocationsForNKnots(List<Vector2D> locations, int numFollowers)
        {
            var knotLocations = new List<List<Vector2D>>(numFollowers);
            knotLocations.Add(locations);

            for (var i = 1; i < numFollowers; i++)
            {
                var followerLocations = new List<Vector2D>();

                var followerCurrentLocation = new Vector2D(0, 0);

                followerLocations.Add(followerCurrentLocation);

                for (var j = 0; j < knotLocations[i - 1].Count; j++)
                {
                    var leaderCurrentLocation = knotLocations[i - 1][j];
                    var newFollowerLocation = ComputeTailLocation(followerCurrentLocation, leaderCurrentLocation);

                    followerLocations.Add(newFollowerLocation);

                    followerCurrentLocation = newFollowerLocation;
                }

                knotLocations.Add(followerLocations);
            }

            var finalTailLocations = knotLocations.Last();

            return finalTailLocations;
        }

        private static List<Vector2D> ComputeTailLocationsFromMoves(IEnumerable<Vector2D> moves)
        {
            var head = new Vector2D(0, 0);
            var tail = new Vector2D(0, 0);

            var tailVisits = new List<Vector2D>();
            tailVisits.Add(tail);

            foreach (var move in moves)
            {
                var newHeadLocation = head + move;
                var newTailLocation = ComputeTailLocation(tail, newHeadLocation);

                tailVisits.Add(newTailLocation);

                head = newHeadLocation;
                tail = newTailLocation;
            }

            return tailVisits;
        }

        private static Vector2D ComputeTailLocation(Vector2D tail, Vector2D head)
        {
            if (tail.IsAdjacent(head))
            {
                return tail;
            }
            else if (tail.IsOnSameRow(head))
            {
                return new Vector2D((head.X + tail.X)/2, tail.Y);
            }
            else if (tail.IsOnSameColumn(head))
            {
                return new Vector2D(tail.X, (head.Y + tail.Y)/2);
            }
            else if (Math.Abs(tail.X - head.X) == 1)
            {
                return new Vector2D(head.X, (head.Y + tail.Y)/2);
            }
            else if (Math.Abs(tail.Y - head.Y) == 1)
            {
                return new Vector2D((head.X + tail.X)/2, head.Y);
            }
            else
            {
                return new Vector2D((head.X + tail.X) / 2, (head.Y + tail.Y) / 2);
            }
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
