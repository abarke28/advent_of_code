using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_23
{
    // https://adventofcode.com/2023/day/23
    public class Day_23 : ISolver
    {
        private const char Path = '.';
        private const char Forest = '#';

        private static readonly Dictionary<char, Vector2D> SlopeMap = new Dictionary<char, Vector2D>
        {
            { '^', Vector2D.Up },
            { '<', Vector2D.Left },
            { '>', Vector2D.Right },
            { 'v', Vector2D.Down }
        };

        private static readonly HashSet<char> Slopes = SlopeMap.Select(s => s.Key).ToHashSet();

        public object Part1(IList<string> lines)
        {
            var map = Grid<char>.FromStrings(lines, c => c);

            var start = map.GetRowVectors(map.Height - 1).Single(v => map.GetValue(v).Equals(Path)) + Vector2D.Down;
            var end = map.GetRowVectors(0).Single(v => map.GetValue(v).Equals(Path));

            var paths = FindPaths(map, start, end);

            var pathLengths = paths.Select(p => p.Count).ToList();

            return pathLengths.Max();
        }

        public object Part2(IList<string> lines)
        {
            var map = Grid<char>.FromStrings(lines, c => c);

            var slopes = map.FindAll(c => Slopes.Contains(c));

            foreach (var slope in slopes)
            {
                map.SetValue(slope, Path);
            }

            var start = map.GetRowVectors(map.Height - 1).Single(v => map.GetValue(v).Equals(Path)) + Vector2D.Down;
            var end = map.GetRowVectors(0).Single(v => map.GetValue(v).Equals(Path));

            var graph = ParseGraph(map, start, end);

            var paths = FindPaths(graph, start, end);

            var scores = paths.Select(p => CalculatePathCost(p, graph) - 1);

            var max = scores.Max();

            return max;
        }

        private static IEnumerable<List<Vector2D>> FindPaths(
            Grid<char> map, Vector2D start, Vector2D goal)
        {
            var initialVisited = new HashSet<Vector2D>((start + Vector2D.Up).AsList());
            var paths = new List<List<Vector2D>>();
            var initialPath = start.AsList();

            var pathsToProcess = new Stack<(List<Vector2D> CurrentPath, HashSet<Vector2D> Visited)>();

            pathsToProcess.Push(new(initialPath, initialVisited));

            while (pathsToProcess.Count > 0)
            {
                var currentState = pathsToProcess.Pop();
                var currentPath = currentState.CurrentPath;
                var visited = currentState.Visited.ToHashSet();

                var currentPosition = currentPath.Last();
                visited.Add(currentPosition);

                if (currentPosition == goal)
                {
                    paths.Add(currentPath);
                    continue;
                }

                var nextPosibleSteps = new List<Vector2D>();

                var current = map.GetValue(currentPosition);

                if (SlopeMap.TryGetValue(current, out var slope))
                {
                    nextPosibleSteps.Add(currentPosition + slope);
                }
                else
                {
                    var adjacents = currentPosition
                        .Get4Neighbors()
                        .Where(n => map.GetValue(n) != Forest);

                    nextPosibleSteps.AddRange(adjacents);
                }

                var validNextPossibleSteps = nextPosibleSteps.Where(nps => !visited.Contains(nps));

                if (!validNextPossibleSteps.Any())
                {
                    continue;
                }

                foreach (var validNextPossibleStep in validNextPossibleSteps)
                {
                    var newPath = currentPath.ToList();
                    newPath.Add(validNextPossibleStep);

                    var newState = (newPath, visited);
                    pathsToProcess.Push(newState);
                }
            }

            return paths;
        }

        private static IEnumerable<List<Vector2D>> FindPaths(
            NodeGraph<Vector2D> map, Vector2D start, Vector2D goal)
        {
            var junctionBeforeGoal = map.GetAdjacentNodes(goal).Single(n => n != goal);
            var costForFinalSegment = map.GetEdgeWeight(junctionBeforeGoal, goal);

            var initialVisited = new HashSet<Vector2D>((start).AsList());
            var paths = new List<List<Vector2D>>();
            var initialPath = start.AsList();

            var pathsToProcess = new Stack<(List<Vector2D> CurrentPath, HashSet<Vector2D> Visited)>();

            pathsToProcess.Push(new(initialPath, initialVisited));

            while (pathsToProcess.Count > 0)
            {
                var currentState = pathsToProcess.Pop();
                var currentPath = currentState.CurrentPath;
                var visited = currentState.Visited.ToHashSet();

                var currentPosition = currentPath.Last();
                visited.Add(currentPosition);

                if (currentPosition == junctionBeforeGoal)
                {
                    currentPath.Add(goal);
                    paths.Add(currentPath);
                    continue;
                }

                var nextPossibleSteps = map.GetAdjacentNodes(currentPosition);
                var validNextPossibleSteps = nextPossibleSteps.Where(nps => !visited.Contains(nps));

                if (!validNextPossibleSteps.Any())
                {
                    continue;
                }

                foreach (var validNextPossibleStep in validNextPossibleSteps)
                {
                    var newPath = currentPath.ToList();
                    newPath.Add(validNextPossibleStep);

                    var newState = (newPath, visited);
                    pathsToProcess.Push(newState);
                }
            }

            return paths;
        }

        private static int CalculatePathCost(List<Vector2D> path, NodeGraph<Vector2D> map)
        {
            var cost = 0;

            for (int i = 0; i < path.Count - 1; i++)
            {
                cost += map.GetEdgeWeight(path[i], path[i + 1]);
            }

            return cost;
        }

        private static NodeGraph<Vector2D> ParseGraph(Grid<char> map, Vector2D start, Vector2D end)
        {
            var junctions = map
                .FindAll(v => map.GetValue(v).Equals(Path) && v.Get4Neighbors().Count(n => map.IsInBounds(n) &&
                                                                                           map.GetValue(n).Equals(Path)) > 2)
                .Union(new List<Vector2D> { start, end })
                .ToList();

            var graph = new NodeGraph<Vector2D>(junctions.Count);

            foreach (var junction in junctions)
            {
                graph.AddNode(junction);

                var pathsFromJunction = junction.Get4Neighbors().Where(n => map.IsInBounds(n) && map.GetValue(n).Equals(Path));

                foreach (var pathFromJunction in pathsFromJunction)
                {
                    var visited = new HashSet<Vector2D>(junction.AsList());
                    var current = pathFromJunction;
                    var pathLength = 1;

                    var optionsFromCurrent = current
                        .Get4Neighbors()
                        .Where(n => map.IsInBounds(n) && map.GetValue(n).Equals(Path) && !visited.Contains(n));

                    while (optionsFromCurrent.Count() == 1)
                    {
                        pathLength++;
                        visited.Add(current);
                        current = optionsFromCurrent.Single();

                        optionsFromCurrent = current
                            .Get4Neighbors()
                            .Where(n => map.IsInBounds(n) && map.GetValue(n).Equals(Path) && !visited.Contains(n));
                    }

                    graph.AddNode(current);
                    graph.AddEdge(junction, current, pathLength, directed: false);
                }
            }

            return graph;
        }
    }
}
