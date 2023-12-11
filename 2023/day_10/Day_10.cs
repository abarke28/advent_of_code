using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2023.day_10
{
    // https://adventofcode.com/2023/day/10
    public class Day_10 : ISolver
    {
        private const char Start = 'S';

        public object Part1(IList<string> lines)
        {
            var grid = Grid<char>.FromStrings(lines, c => c);
            
            var start = grid.FindAll(v => grid.GetValue(v) == Start).Single();

            var possibleDirections = start.Get4Neighbors();

            var lengths = new List<int>();
            foreach (var possibleDirection in possibleDirections)
            {
                var length = GetPipeLoopLength(start, possibleDirection, grid, out _);
                lengths.Add(length);
            }

            return lengths.Select(l => Math.Ceiling(l / 2.0)).Max();
        }

        public object Part2(IList<string> lines)
        {
            var grid = Grid<char>.FromStrings(lines, c => c);

            var start = grid.FindAll(v => grid.GetValue(v) == Start).Single();

            var possibleDirections = start.Get4Neighbors();

            var loop = new List<Vector2D>();

            foreach (var possibleDirection in possibleDirections)
            {
                var length = GetPipeLoopLength(start, possibleDirection, grid, out var candidateLoop);

                if (length > loop.Count) loop = candidateLoop;
            }

            var interiorSize = CalculateLoopInterior(grid, loop);

            return interiorSize;
        }

        private static int CalculateLoopInterior(Grid<char> map, List<Vector2D> loop)
        {
            // Shoelace Formula:
            // A = 1/2 (v0 ^ v1 + v1 ^ v2 + ... vn-1 ^ v0)
            // Where A ^ B = A.x * B.y - B.x * A.y
            var shoeLaceSum = 0;

            while (shoeLaceSum <= 0)
            {
                for (int i = 0; i < loop.Count - 1; i++)
                {
                    var v1 = loop[i];
                    var v2 = loop[i + 1];

                    var wedge = v1.X * v2.Y - v2.X * v1.Y;

                    shoeLaceSum += wedge;
                }

                var finalShoeLace = loop.Last().X * loop.First().Y - loop.Last().Y * loop.First().X;

                shoeLaceSum += finalShoeLace;
                shoeLaceSum /= 2;

                if (shoeLaceSum > 0)
                {
                    break;
                }
                else
                {
                    // Shoelace Forumla needs to run counter clockwise, just try both ways.
                    shoeLaceSum = 0;
                    loop.Reverse();
                }
            }

            // Pick's Theorem:
            // A = i + b/2 - 1
            // => i = A + 1 - b/2

            var picksInteriorPoints = shoeLaceSum + 1 - loop.Count / 2;

            return picksInteriorPoints;
        }

        private static int GetPipeLoopLength(Vector2D startingPoint, Vector2D firstStep, Grid<char> map, out List<Vector2D> loopPipeSegments)
        {
            var currentLocation = startingPoint;
            var nextLocation = firstStep;
            var nextDirection = Vector2D.Zero;
            var length = 0;

            loopPipeSegments = new List<Vector2D> { currentLocation };

            while (nextLocation != startingPoint)
            {
                if (!CanFollowPipeSegment(currentLocation, nextLocation, map)) return 0;

                length++;

                nextDirection = ComputeNextDirection(currentLocation, nextLocation, map.GetValue(nextLocation));
                currentLocation = nextLocation;
                nextLocation = nextDirection;

                loopPipeSegments.Add(currentLocation);
            }

            return length;
        }

        private static Vector2D ComputeNextDirection(Vector2D currentLocation, Vector2D targetLocation, char targetPipe)
        {
            switch (targetPipe)
            {
                case '|':
                    return new Vector2D(currentLocation.X, targetLocation.Y + (targetLocation.Y - currentLocation.Y));
                case '-':
                    return new Vector2D(targetLocation.X + (targetLocation.X - currentLocation.X), targetLocation.Y);
                case 'L':
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Right : targetLocation + Vector2D.Up;
                case 'J':
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Left : targetLocation + Vector2D.Up;
                case '7':
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Left : targetLocation + Vector2D.Down;
                case 'F':
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Right : targetLocation + Vector2D.Down;
                case 'S':
                    return targetLocation;
                case '.':
                default:
                    throw new Exception("Cannot navigate");
            }
        }

        private static bool CanFollowPipeSegment(Vector2D currentLocation, Vector2D nextLocation, Grid<char> map)
        {
            if (!map.IsInBounds(nextLocation)) return false;

            var nextPipe = map[nextLocation];

            switch (nextPipe)
            {
                case '.': 
                    return false;
                case '|':
                    return currentLocation.IsOnSameColumn(nextLocation);
                case '-':
                    return currentLocation.IsOnSameRow(nextLocation);
                case 'L':
                    return (currentLocation + Vector2D.Down == nextLocation) || (currentLocation +Vector2D.Left == nextLocation);
                case 'J':
                    return (currentLocation + Vector2D.Down == nextLocation) || (currentLocation +Vector2D.Right == nextLocation);
                case '7':
                    return (currentLocation + Vector2D.Up == nextLocation) || (currentLocation + Vector2D.Right == nextLocation);
                case 'F':
                    return (currentLocation + Vector2D.Up == nextLocation) || (currentLocation + Vector2D.Left == nextLocation);
                case 'S':
                    return true;
                default:
                    throw new Exception($"Unknown map character: {nextPipe}");
            }
        }
    }
}
