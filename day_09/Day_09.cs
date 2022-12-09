using aoc.common;
using aoc.utils;

namespace aoc.day_09
{
    // https://adventofcode.com/2022/day/09
    public class Day_09 : ISolver
    {
        private enum Direction
        {
            Right = 'R',
            Left = 'L',
            Up = 'U',
            Down = 'D'
        }

        private class RopeInstruction
        {
            public Direction Direction { get; set; }
            public int Count { get; set; }

            public static RopeInstruction FromString(string s)
            {
                var words = s.Split(' ');

                return new RopeInstruction
                {
                    Direction = ParseStringToDirection(words[0]),
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
        }

        private static List<Coordinate> ComputeTailLocationsFromMoves(IEnumerable<Direction> moves)
        {
            //var knots = new List<Coordinate>(Enumerable.Repeat<Coordinate>(new Coordinate(0, 0), numKnots));

            var head = new Coordinate(0, 0);
            var tail = new Coordinate(0, 0);

            var tailVisits = new List<Coordinate>();
            tailVisits.Add(tail);

            foreach (var move in moves)
            {
                var newHeadLocation = ComputeHeadLocation(head, move);
                var newTailLocation = ComputeTailLocation(tail, newHeadLocation);

                tailVisits.Add(newTailLocation);

                head = newHeadLocation;
                tail = newTailLocation;
            }

            return tailVisits;
        }

        private static Coordinate ComputeHeadLocation(Coordinate startingLocation, Direction move)
        {
            var x = startingLocation.X;
            var y = startingLocation.Y;

            switch (move)
            {
                case Direction.Left:
                    return new Coordinate(x - 1, y);
                case Direction.Right:
                    return new Coordinate(x + 1, y);
                case Direction.Up:
                    return new Coordinate(x, y + 1);
                case Direction.Down:
                    return new Coordinate(x, y - 1);
                default:
                    throw new ArgumentException(nameof(move));
            }
        }

        private static Coordinate ComputeTailLocation(Coordinate tail, Coordinate head)
        {
            if (tail.IsTouching(head))
            {
                return tail;
            }
            else
            {
                if (tail.IsOnSameRow(head))
                {
                    return new Coordinate((head.X + tail.X)/2, tail.Y);
                }

                if (tail.IsOnSameColumn(head))
                {
                    return new Coordinate(tail.X, (head.Y + tail.Y)/2);
                }

                // Head must be diagnol away from tail
                if (Math.Abs(tail.X - head.X) == 1)
                {
                    return new Coordinate(head.X, (head.Y + tail.Y)/2);
                }
                else
                {
                    return new Coordinate((head.X + tail.X)/2, head.Y);
                }
            }
        }

        private static IEnumerable<Direction> ParseInstructionToMoves(RopeInstruction instruction)
        {
            var moves = new List<Direction>();

            while (instruction.Count-- > 0)
            {
                moves.Add(instruction.Direction);
            }

            return moves;
        }

        private static Direction ParseStringToDirection(string s)
        {
            switch (s)
            {
                case "U":
                    return Direction.Up;
                case "D":
                    return Direction.Down;
                case "L":
                    return Direction.Left;
                case "R":
                    return Direction.Right;
                default:
                    throw new ArgumentException(nameof(s));
            }
        }
    }
}
