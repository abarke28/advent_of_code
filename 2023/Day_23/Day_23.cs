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

            var start = map.GetRowVectors(map.Height - 1).Single(v => map.GetValue(v).Equals(Path));
            var end = map.GetRowVectors(0).Single(v => map.GetValue(v).Equals(Path));

            var paths = FindPaths(map, start.AsList(), end, new HashSet<Vector2D>());

            var pathLengths = paths.Select(p => p.Count - 1).ToList();

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

            var start = map.GetRowVectors(map.Height - 1).Single(v => map.GetValue(v).Equals(Path));
            var end = map.GetRowVectors(0).Single(v => map.GetValue(v).Equals(Path));

            var paths = FindPaths(map, start.AsList(), end, new HashSet<Vector2D>());

            var pathLengths = paths.Select(p => p.Count - 1).ToList();

            return pathLengths.Max();
        }

        private static IEnumerable<List<Vector2D>> FindPaths(
            Grid<char> map, List<Vector2D> currentPath, Vector2D goal, HashSet<Vector2D> visited)
        {
            var paths = new List<List<Vector2D>>();

            var currentPosition = currentPath.Last();
            var newVisited = visited.ToHashSet();
            newVisited.Add(currentPosition);

            if (currentPosition == goal)
            {
                paths.Add(currentPath);
                return paths;
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
                    .Where(n => map.IsInBounds(n) && map.GetValue(n) != Forest);

                nextPosibleSteps.AddRange(adjacents);
            }

            var validNextPossibleSteps = nextPosibleSteps.Where(nps => !visited.Contains(nps));

            if (!validNextPossibleSteps.Any())
            {
                return paths;
            }

            foreach (var validNextPossibleStep in validNextPossibleSteps)
            {
                var newPath = currentPath.ToList();
                newPath.Add(validNextPossibleStep);

                paths.AddRange(FindPaths(map, newPath, goal, newVisited));
            }

            return paths;
        }
    }
}
