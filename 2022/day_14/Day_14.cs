using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_14
{
    // https://adventofcode.com/2022/day/14
    public class Day_14
    {
        private static readonly Vector2D SandOrigin = new Vector2D(500, 0);
        private static readonly List<Vector2D> SandMoves = new List<Vector2D>
        {
            Vector2D.Up,
            new Vector2D(-1, 1),
            new Vector2D(1, 1)
        };

        private class SandPath
        {
            public List<Vector2D> PathNodes = new List<Vector2D>();
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_14/input.txt");

            var sandPaths = GetSandPaths(lines);
            var sandPoints = sandPaths.SelectMany(sp => GenerateSandPoints(sp)).ToHashSet();

            var lowestPoint = sandPoints.Select(sp => sp.Y).Max();

            var numSand = CountAmountOfSandBeforeOverflow(SandOrigin, sandPoints, v => v.Y > lowestPoint);
            Console.WriteLine(numSand);

            var sandPoints2 = sandPaths.SelectMany(sp => GenerateSandPoints(sp)).ToHashSet();
            var numSand2 = CountAmountOfSandBeforeOverflow(SandOrigin,
                                                           sandPoints2,
                                                           v => v == SandOrigin,
                                                           lowestPoint + 2);
            Console.WriteLine(numSand2 );
        }

        private static int CountAmountOfSandBeforeOverflow(Vector2D start,
                                                           HashSet<Vector2D> currentSand,
                                                           Func<Vector2D, bool> fullPredicate,
                                                           int? maxDepth = null)
        {
            var sandAmount = 0;

            while (true)
            {
                Vector2D sandRestingPoint = SimulateSandFall(start, currentSand, fullPredicate, maxDepth);

                if (sandRestingPoint == Vector2D.Down)
                {
                    return sandAmount;
                }
                else
                {
                    currentSand.Add(sandRestingPoint);
                    sandAmount++;

                    if (fullPredicate(sandRestingPoint))
                    {
                        return sandAmount;
                    }
                }
            }
        }

        private static Vector2D SimulateSandFall(Vector2D start,
                                                 HashSet<Vector2D> currentSand,
                                                 Func<Vector2D, bool> fullPredicate,
                                                 int? maxDepth = null)
        {
            var currentSandLocation = start;

            while (SandCanMove(currentSandLocation, currentSand, out var nextLocation, maxDepth))
            {
                currentSandLocation = nextLocation;

                if (fullPredicate(currentSandLocation))
                {
                    return Vector2D.Down;
                }
            }

            return currentSandLocation;
        }

        private static bool SandCanMove(Vector2D currentLocation,
                                        HashSet<Vector2D> currentSand,
                                        out Vector2D nextLocation,
                                        int? maxDepth = null)
        {
            foreach (var sandMove in SandMoves)
            {
                var locationCandidate = currentLocation + sandMove;

                if (maxDepth.HasValue && locationCandidate.Y == maxDepth.Value)
                {
                    nextLocation = currentLocation;
                    return false;
                }

                if (!currentSand.Contains(locationCandidate))
                {
                    nextLocation = locationCandidate;
                    return true;
                }
            }

            nextLocation = Vector2D.Zero;
            return false;
        }

        private static IEnumerable<SandPath> GetSandPaths(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var pathNodes = line.Split("->")
                                    .Select(p => p.Trim())
                                    .Select(p => p.ReadAllNumbers(',').ToList())
                                    .Select(pc => new Vector2D(pc[0], pc[1]));

                yield return new SandPath { PathNodes = pathNodes.ToList() };
            }
        }

        private static IEnumerable<Vector2D> GenerateSandPoints(SandPath path)
        {
            var points = new List<Vector2D>();

            for (var i = 0; i < path.PathNodes.Count - 1; i++)
            {
                var start = path.PathNodes[i];
                var end = path.PathNodes[i + 1];

                var delta = Vector2D.Normalize(end - start);
                var current = start;
                points.Add(current);

                while (current != end)
                {
                    current += delta;
                    points.Add(current);
                }
            }

            return points;
        }
    }
}
