using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_18
{
    // https://adventofcode.com/2023/day/18
    public class Day_18 : ISolver
    {
        private class Instruction
        {
            public int Count { get; set; }
            public Vector2D Direction { get; set; }

            public Instruction(int count, Vector2D direction)
            {
                Count = count;
                Direction = direction;
            }
        }

        public object Part1(IList<string> lines)
        {
            var instructions = GetInstructions(lines);

            var startingPoint = Vector2D.Zero;

            var path = ComputePath(startingPoint, instructions).ToList();
            var distinctPoints = path.Distinct().ToList();

            var area = distinctPoints.CalculatePolygonArea();
            var internalArea = area - distinctPoints.Count / 2 + 1;

            var pitArea = internalArea + distinctPoints.Count;

            return pitArea;
        }

        public object Part2(IList<string> lines)
        {
            var instructions = GetCorrectedInstructions(lines);

            var startingPoint = Vector2D.Zero;

            var path = ComputePath(startingPoint, instructions);
            var vertices = ComputeVertices(startingPoint, instructions);

            var area = vertices.CalculatePolygonArea();
            var internalArea = area - (path.Count - 1) / 2 + 1;

            var pitArea = internalArea + (path.Count - 1);

            return pitArea;
        }

        private static List<Vector2D> ComputeVertices(Vector2D startingPoint, IEnumerable<Instruction> instructions)
        {
            var path = new List<Vector2D>
            {
                startingPoint
            };

            var currentLocation = startingPoint;

            foreach (var instruction in instructions)
            {
                var nextLocation = currentLocation + (instruction.Count * instruction.Direction);

                path.Add(nextLocation);

                currentLocation = nextLocation;
            }

            return path;
        }

        private static List<Vector2D> ComputePath(Vector2D startingPoint, IEnumerable<Instruction> instructions)
        {
            var path = new List<Vector2D>
            {
                startingPoint
            };

            var currentLocation = startingPoint;

            foreach (var instruction in instructions)
            {
                var count = instruction.Count;

                while (count > 0)
                {
                    var nextLocation = currentLocation + instruction.Direction;

                    path.Add(nextLocation);

                    currentLocation = nextLocation;
                    count--;
                }
            }

            return path;
        }

        private static IEnumerable<Instruction> GetInstructions(IList<string> lines)
        {
            var directionDict = new Dictionary<string, Vector2D>
            {
                { "U", Vector2D.Up },
                { "L", Vector2D.Left },
                { "D", Vector2D.Down },
                { "R", Vector2D.Right },
            };

            foreach (var line in lines)
            {
                var words = line.Split(' ');

                yield return new Instruction(int.Parse(words[1]), directionDict[words[0]]);
            }
        }

        private static IEnumerable<Instruction> GetCorrectedInstructions(IList<string> lines)
        {
            var directionDict = new Dictionary<char, Vector2D>
            {
                { '3', Vector2D.Up },
                { '2', Vector2D.Left },
                { '1', Vector2D.Down },
                { '0', Vector2D.Right },
            };

            foreach (var line in lines)
            {
                var words = line.Split(' ');

                var color = words.Last()[2..^1];

                var direction = directionDict[color[^1]];
                var hexString = color[0..5];
                var hexNumber = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);

                yield return new Instruction(hexNumber, direction);
            }
        }
    }
}
