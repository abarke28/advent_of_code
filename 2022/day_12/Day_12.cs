using aoc.common;
using aoc.utils;

namespace aoc.y2022.day_12
{
    // https://adventofcode.com/2022/day/12
    public class Day_12 : ISolver
    {
        private const char BestSignal = 'E';
        private const char Start = 'S';
        private const char Min = 'a';

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_12/input.txt");

            var grid = Grid<char>.FromStrings(lines, c => c);

            grid.TryFind(Start, out var start);
            grid.TryFind(BestSignal, out var end);

            var endNodeNum = GenerateNodeNumber(end.X, end.Y, grid.Width);
            var startNodeNum = GenerateNodeNumber(start.X, start.Y, grid.Width);

            var graph = Graph.FromGrid(grid, GetNeighbors, (from, to) => CanTraverse(from, to) ? 1 : 0);

            var distances = graph.GetMinDistances(startNodeNum);

            Console.WriteLine(distances[endNodeNum]);

            var graph2 = Graph.FromGrid(grid, GetNeighbors, (from, to) => CanTraverse2(from, to) ? 1 : 0);

            var distances2 = graph2.GetMinDistances(endNodeNum);

            var allPoints = grid.GetAllPoints();

            var min = int.MaxValue;
            foreach(var p in allPoints)
            {
                var pNodeNum = GenerateNodeNumber(p.X, p.Y, grid.Width);

                if (grid.GetValue(p.X, p.Y) == Min)
                {
                    // TODO - There is (possibly) some bug in the D. Algo is overflowing occasionally.
                    // Potentially just a very bad path though?
                    if (distances2[pNodeNum] < 0)
                    {
                        continue;
                    }

                    min = Math.Min(min, distances2[pNodeNum]);
                }
            }

            Console.WriteLine(min);
        }

        private static IEnumerable<Vector2D> GetNeighbors(Vector2D v)
        {
            return new List<Vector2D>
            {
                v + Vector2D.Up,
                v + Vector2D.Down,
                v + Vector2D.Left,
                v + Vector2D.Right
            };
        }

        private static bool CanTraverse(char from, char to)
        {
            var fromScore = GetLetterScore(from);
            var toScore = GetLetterScore(to);

            return toScore - fromScore <= 1;
        }

        private static bool CanTraverse2(char from, char to)
        {
            var fromScore = GetLetterScore(from);
            var toScore = GetLetterScore(to);

            return fromScore - toScore <= 1;
        }

        private static int GetLetterScore(char c)
        {
            if (c == BestSignal) { return 26; }
            if (c == Start) { return 1; }

            const int lowerCaseOffset = 96;

            var asciiCode = (int)c - lowerCaseOffset;

            return asciiCode;
        }

        private static int GenerateNodeNumber(int x, int y, int gridWidth)
        {
            return x + (y * gridWidth);
        }
    }
}
