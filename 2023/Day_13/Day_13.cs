using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_13
{
    // https://adventofcode.com/2023/day/13
    public class Day_13 : ISolver
    {
        private const char Hash = '#';
        private const char Dot = '.';

        private const int HorizontalScoreFactor = 100;

        public object Part1(IList<string> lines)
        {
            var maps = ParseLines(lines);

            var scores = maps.Select(m => GetReflectionScore(m));
            var sum = scores.Sum(s => s.Score);

            return sum;
        }

        public object Part2(IList<string> lines)
        {
            var maps = ParseLines(lines);
            var mapsAndOldIndexes = maps.Select(m => (Map: m, RowIndex: FindReflectionRowIndex(m), ColumnIndex: FindReflectionColumnIndex(m)));

            var sum = 0L;
            foreach (var mapAndIndexes in mapsAndOldIndexes)
            {
                var (smudgePoint, score) = FindSmudge(mapAndIndexes.Map, mapAndIndexes.RowIndex, mapAndIndexes.ColumnIndex);

                sum += score;
            }

            return sum;
        }

        private static (Vector2D SmudgePoint, int Score) FindSmudge(Grid<char> map, int oldRowIndex, int oldColumnIndex)
        {
            var foundOldRowIndex = false;
            var foundOldColumnIndex = false;

            for (var x = 0; x < map.Width; x++)
            {
                for (var y = map.Height - 1; y >= 0; y--)
                {
                    var v = new Vector2D(x, y);

                    var swappedMap = map.Copy();
                    swappedMap.SetValue(v, swappedMap[v] == Hash ? Dot : Hash);

                    var rowIndex = FindReflectionRowIndex(swappedMap);
                    if (rowIndex > 0)
                    {
                        if (rowIndex == oldRowIndex)
                        {
                            foundOldRowIndex = true;
                        }
                        else
                        {
                            map.PrintGrid();
                            swappedMap.PrintGrid();
                            Console.WriteLine($"Row index: {rowIndex} - Smudge Point: {v} - Score: {rowIndex * HorizontalScoreFactor}\n");
                            return (v, rowIndex * HorizontalScoreFactor);
                        }
                    }

                    var columnIndex = FindReflectionColumnIndex(swappedMap);
                    if (columnIndex > 0 && columnIndex != oldColumnIndex)
                    {
                        if (columnIndex == oldColumnIndex)
                        {
                            foundOldColumnIndex = true;
                        }
                        else
                        {
                            map.PrintGrid();
                            swappedMap.PrintGrid();
                            Console.WriteLine($"Column index: {columnIndex} - Smudge Point: {v} - Score: {columnIndex}\n");
                            return (v, columnIndex);
                        }
                    }
                }
            }

            if (foundOldColumnIndex) return (Vector2D.Zero, oldColumnIndex);
            if (foundOldRowIndex) return (Vector2D.Zero, oldRowIndex * HorizontalScoreFactor);

            throw new Exception("No smudge point found");
        }

        private static (int Score, int RowIndex, int ColumnIndex) GetReflectionScore(Grid<char> map)
        {
            var columnIndex = FindReflectionColumnIndex(map);

            if (columnIndex > 0)
            {
                return (columnIndex, -1, columnIndex);
            }

            var rowIndex = FindReflectionRowIndex(map);

            if (rowIndex > 0)
            {
                return (rowIndex * HorizontalScoreFactor, rowIndex, -1);
            }

            throw new Exception($"No reflection row or column found.");
        }

        private static int FindReflectionColumnIndex(Grid<char> map)
        {
            for (int i = 1; i < map.Width; i++)
            {
                for (int c = 0; c < (map.Width / 2) + 1; c ++)
                {
                    var column1Index = i - 1 - c;
                    var column2Index = i + c;

                    if (column1Index < 0 || column2Index >= map.Width)
                    {
                        return i;
                    }

                    var column1 = map.GetColumn(column1Index);
                    var column2 = map.GetColumn(column2Index);

                    if (!column1.SequenceEqual(column2))
                    {
                        break;
                    }
                }
            }

            return -1;
        }

        private static int FindReflectionRowIndex(Grid<char> map)
        {
            for (int i = 1; i < map.Height; i++)
            {
                for (int r = 0; r < (map.Height / 2) + 1; r++)
                {
                    var row1Index = i - 1 - r;
                    var row2Index = i + r;

                    if (row1Index < 0 || row2Index >= map.Height)
                    {
                        // Rows are indexed backwards
                        return map.Height - i;
                    }

                    var row1 = map.GetRow(row1Index);
                    var row2 = map.GetRow(row2Index);

                    if (!row1.SequenceEqual(row2))
                    {
                        break;
                    }
                }
            }

            return -1;
        }

        private static IEnumerable<Grid<char>> ParseLines(IList<string> lines)
        {
            var maps = lines
                .ChunkBy(l => !string.IsNullOrWhiteSpace(l))
                .Select(ls => ls.ToList())
                .Select(ls => Grid<char>.FromStrings(ls, c => c));

            return maps;
        }
    }
}
