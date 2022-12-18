using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2021.day_13
{
    // https://adventofcode.com/2021/day/13
    public class Day_13 : ISolver
    {
        private enum FoldDirection
        {
            Up,
            Left
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_13/input.txt");

            var dots = GetDots(lines).ToHashSet();
            var directions = GetDirections(lines).ToList();

            ExecuteFold(dots, directions.First().Direction, directions.First().At);
            Console.WriteLine(dots.Count);

            for (var i = 1; i < directions.Count; i++)
            {
                ExecuteFold(dots, directions[i].Direction, directions[i].At);
            }

            PrintDots(dots);
        }

        private static void ExecuteFold(HashSet<Vector2D> dots, FoldDirection direction, int at)
        {
            if (direction == FoldDirection.Left)
            {
                var affectedDots = dots.Where(d => d.X > at).ToList();
                dots.RemoveWhere(d => d.X > at);

                var newDotLocations = affectedDots.Select(d => new Vector2D(at - (d.X - at), d.Y));
                dots.AddRange(newDotLocations);
            }
            else if (direction == FoldDirection.Up)
            {
                var affectedDots = dots.Where(d => d.Y > at).ToList();
                dots.RemoveWhere(d => d.Y > at);

                var newDotLocations = affectedDots.Select(d => new Vector2D(d.X, at - (d.Y - at)));
                dots.AddRange(newDotLocations);
            }
            else
            {
                throw new Exception("Unexpected fold direction");
            }
        }

        private static void PrintDots(IEnumerable<Vector2D> dots)
        {
            var minX = dots.Min(d => d.X);
            var minY = dots.Min(d => d.Y);

            var translatedDots = dots.Select(d => new Vector2D(d.X - minX, d.Y - minY));

            var maxX = translatedDots.Max(d => d.X);
            var maxY = translatedDots.Max(d => d.Y);

            var grid = new Grid<char>(maxX + 1, maxY + 1, ' ');

            foreach (var dot in translatedDots)
            {
                grid[dot] = '#';
            }

            grid.PrintGrid();
        }

        private static IEnumerable<Vector2D> GetDots(IEnumerable<string> lines)
        {
            var dotLines = lines.TakeWhile(l => !string.IsNullOrWhiteSpace(l));
            var dotNums = dotLines.Select(dl => dl.ReadAllNumbers(',').ToList());
            var dots = dotNums.Select(d => new Vector2D(d[0], d[1]));


            return dots;
        }

        private static IEnumerable<(FoldDirection Direction, int At)> GetDirections(IEnumerable<string> lines)
        {
            return lines.Where(l => l.Contains("fold"))
                        .Select(l => l.Split('=').ToList())
                        .Select(l => (Direction: l[0].Contains('y') ? FoldDirection.Up : FoldDirection.Left, At: int.Parse(l[1])));
        }
    }
}
