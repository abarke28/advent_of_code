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

            ReplaceStartingPoint(grid, loop);
            var interiorSize = CalculateLoopInterior(grid, loop.ToHashSet());

            return interiorSize;
        }

        private static int CalculateLoopInterior(Grid<char> map, HashSet<Vector2D> loopPipeLocations)
        {
            var potentialInternalPoints = map.GetAllPoints().Where(p => !loopPipeLocations.Contains(p));
            var internalPoints = new HashSet<Vector2D>();


            foreach (var potentialPoint in potentialInternalPoints)
            {
                var charCountsToLeft = map
                    .FindAll(p => p.X < potentialPoint.X && p.Y == potentialPoint.Y)
                    .Where(p => loopPipeLocations.Contains(p))
                    .Select(p => map.GetValue(p))
                    .GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count());

                if (charCountsToLeft.Any())
                {
                    var verticalPipeCount = charCountsToLeft.TryGetValue('|', out var vert) ? vert : 0;
                    var fCount = charCountsToLeft.TryGetValue('F', out var q1) ? q1 : 0;
                    var sevCount = charCountsToLeft.TryGetValue('7', out var q2) ? q2 : 0;
                    
                    var totalCrossings = verticalPipeCount + fCount + sevCount;

                    if (totalCrossings.IsOdd())
                    {
                        internalPoints.Add(potentialPoint);
                    }
                }
            }

            return internalPoints.Count;
        }

        private static void ReplaceStartingPoint(Grid<char> map, List<Vector2D> loop)
        {
            var startingPoint = loop.First();

            var firstStep = loop.Skip(1).Take(1).Single();
            var lastStep = loop.Last();

            var delta = lastStep - firstStep;

            switch (delta.X, delta.Y)
            {
                case (0, 1):
                case (0, -1):
                    map.SetValue(startingPoint, '|');
                    break;
                case (1, 0):
                case (-1, 0):
                    map.SetValue(startingPoint, '-');
                    break;
                case (1, -1):
                    map.SetValue(startingPoint, 'F');
                    break;
                case (1, 1):
                    map.SetValue(startingPoint, 'L');
                    break;
                case (-1, -1):
                    map.SetValue(startingPoint, '7');
                    break;
                case (-1, 1):
                    map.SetValue(startingPoint, 'J');
                    break;

            }
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
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Right : targetLocation + Vector2D.Down;
                case 'J':
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Left : targetLocation + Vector2D.Down;
                case '7':
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Left : targetLocation + Vector2D.Up;
                case 'F':
                    return currentLocation.IsOnSameColumn(targetLocation) ? targetLocation + Vector2D.Right : targetLocation + Vector2D.Up;
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
                    return (currentLocation + Vector2D.Up == nextLocation) || (currentLocation +Vector2D.Left == nextLocation);
                case 'J':
                    return (currentLocation + Vector2D.Up == nextLocation) || (currentLocation +Vector2D.Right == nextLocation);
                case '7':
                    return (currentLocation + Vector2D.Down == nextLocation) || (currentLocation + Vector2D.Right == nextLocation);
                case 'F':
                    return (currentLocation + Vector2D.Down == nextLocation) || (currentLocation + Vector2D.Left == nextLocation);
                case 'S':
                    return true;
                default:
                    throw new Exception($"Unknown map character: {nextPipe}");
            }
        }
    }
}
