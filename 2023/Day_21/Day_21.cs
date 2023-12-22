using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;
using System.Numerics;

namespace Aoc.Y2023.Day_21
{
    // https://adventofcode.com/2023/day/21
    public class Day_21 : ISolver
    {
        private const char Start = 'S';
        private const char Rock = '#';

        public object Part1(IList<string> lines)
        {
            var map = Grid<char>.FromStrings(lines, c => c);

            var start = map.FindAll(c => c == Start).Single();

            var numSteps = 64;

            var visitedAtSteps = GetNumberOfLocations(numSteps, map, start);

            return visitedAtSteps[numSteps];
        }

        public object Part2(IList<string> lines)
        {
            var map = Grid<char>.FromStrings(lines, c => c);

            var start = map.FindAll(c => c == Start).Single();

            var numSteps = 26_501_365L;
            var periodicity = map.Width;
            var offset = numSteps % periodicity;
            var cycles = (numSteps - offset) / periodicity;
            var stepsToSimulate = (int)(offset + periodicity * 3);

            var counts = GetNumberOfInfiniteLocations(stepsToSimulate, map, start);

            var interestingSteps = counts.WithIndex(1).Where(c => (c.Index - (numSteps % periodicity)) % periodicity == 0).Select(c => c.Item).ToList();

            // y = ax^2 + bx + c
            //
            // y'' = 2a = d2 => a = d2/2
            // y2 - y1 = 4a + 2b + c - (a + b + c) => b = y2 - y1 -3a
            // a*(0)^2 + b*(0) + c = y0   => c = y1

            var d1_1 = interestingSteps[2].Value - interestingSteps[1].Value;
            var d1_2 = interestingSteps[3].Value - interestingSteps[2].Value;

            var d2 = d1_2 - d1_1;

            var a = d2 / 2;
            var b = interestingSteps[2].Value - interestingSteps[1].Value - 3 * a;
            var c = interestingSteps[0].Value;

            return a * cycles * cycles + b * cycles + c;
        }

        private static Dictionary<int, int> GetNumberOfLocations(int numSteps, Grid<char> map, Vector2D start)
        {
            var visited = new List<Vector2D>();
            var validPoints = map.FindAll(c => c != Rock).ToHashSet();
            var couldVisitPerStep = Enumerable.Range(1, numSteps).ToDictionary(n => n, _ => 0);

            var toVisit = new Queue<Vector2D>();
            toVisit.Enqueue(start);

            for (int i = 0; i < numSteps; i++)
            {
                var nextSteps = new HashSet<Vector2D>();

                while (toVisit.Count > 0)
                {
                    var current = toVisit.Dequeue();
                    var adjacent = current.Get4Neighbors().Where(n => validPoints.Contains(n)).ToList();

                    nextSteps.AddRange(adjacent);
                }

                foreach (var nextStep in nextSteps)
                {
                    toVisit.Enqueue(nextStep);
                }

                couldVisitPerStep[i + 1] = toVisit.Count;
            }

            return couldVisitPerStep;
        }

        private static Dictionary<int, int> GetNumberOfInfiniteLocations(int numSteps, Grid<char> map, Vector2D start)
        {
            var visited = new List<Vector2D>();
            var couldVisitPerStep = Enumerable.Range(1, numSteps).ToDictionary(n => n, _ => 0);

            var toVisit = new Queue<Vector2D>();
            toVisit.Enqueue(start);

            for (int i = 0; i < numSteps; i++)
            {
                var nextSteps = new HashSet<Vector2D>();

                while (toVisit.Count > 0)
                {
                    var current = toVisit.Dequeue();
                    var adjacent = GetInfiniteNeighbors(current, map).ToList();

                    nextSteps.AddRange(adjacent);
                }

                foreach (var nextStep in nextSteps)
                {
                    toVisit.Enqueue(nextStep);
                }

                couldVisitPerStep[i + 1] = toVisit.Count;
            }

            return couldVisitPerStep;
        }


        private static IEnumerable<Vector2D> GetInfiniteNeighbors(Vector2D position, Grid<char> map)
        {
            var nominalNeighbors = position.Get4Neighbors();

            var validNeighbors = nominalNeighbors
                .Where(n => map.IsInBounds(n.X.MathMod(map.Width), n.Y.MathMod(map.Height)) &&
                            map.GetValue(n.X.MathMod(map.Width), n.Y.MathMod(map.Height)) != Rock);

            return validNeighbors;
        }

        private static Dictionary<int, int> GetPossiblePlots(int numSteps, Grid<char> map, Vector2D start)
        {
            var possiblePoints = map.FindAll(c => c != Rock).ToHashSet();
            var visited = new HashSet<Vector2D>(start.AsList());
            var visitedAtStep = Enumerable.Range(1, numSteps).ToDictionary(n => n, _ => 0);
            var toVisit = new Queue<Vector2D>();

            toVisit.Enqueue(start);

            var currentPosition = start;

            for (int i = 1; i <= numSteps; i++)
            {
                var nodesAtDepth = toVisit.Count;

                while (nodesAtDepth-- > 0)
                {
                    currentPosition = toVisit.Dequeue();

                    var nextPositions = currentPosition
                        .Get4Neighbors()
                        .Where(n => possiblePoints.Contains(n))
                        .Where(n => !visited.Contains(n))
                        .ToList();

                    foreach (var nextPosition in nextPositions)
                    {
                        toVisit.Enqueue(nextPosition);
                    }

                    visited.Add(currentPosition);
                }

                visitedAtStep[i] = visited.Count;
            }

            return visitedAtStep;
        }
    }
}
