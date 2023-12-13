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

            var smudgesAndScores = mapsAndOldIndexes.Select(m => FindSmudge(m.Map, m.RowIndex, m.ColumnIndex)).ToList();
          
            return smudgesAndScores.Select(s => s.Score).Sum();
        }

        private static (Vector2D SmudgePoint, int Score, bool OldLineValid) FindSmudge(Grid<char> map, int oldRowIndex, int oldColumnIndex)
        {
            var foundOldRowIndex = false;
            var foundOldColumnIndex = false;
            var vector = new Vector2D();

            foreach (var v in map.GetAllPoints())
            {
                var swappedMap = map.Copy();
                swappedMap.SetValue(v, swappedMap[v] == Hash ? Dot : Hash);

                // There are two matches.
                var rowIndexAny = FindReflectionRowIndex(swappedMap);
                var rowIndexNew = FindReflectionRowIndex(swappedMap, oldRowIndex);

                if (rowIndexAny > 0 || rowIndexNew > 0)
                {
                    if (rowIndexAny == oldRowIndex && rowIndexNew < 0)
                    {
                        foundOldRowIndex = true;
                        vector = v;
                    }
                    else if (rowIndexNew > 0)
                    {
                        return (v, (map.Height - rowIndexNew) * HorizontalScoreFactor, false);
                    }
                }

                var columnIndexAny = FindReflectionColumnIndex(swappedMap);
                var columnIndexNew = FindReflectionColumnIndex(swappedMap, oldColumnIndex);

                if (columnIndexAny > 0 || columnIndexNew > 0)
                {
                    if (columnIndexAny == oldColumnIndex && columnIndexNew < 0)
                    {
                        foundOldColumnIndex = true;
                        vector = v;
                    }
                    else if (columnIndexNew > 0)
                    {
                        return (v, columnIndexNew, false);
                    }
                }
            }

            if (foundOldColumnIndex) return (vector, oldColumnIndex, true);
            if (foundOldRowIndex) return (vector, (map.Height - oldRowIndex) * HorizontalScoreFactor, true);

            throw new Exception("No smudge point found");
        }

        private static (int Score, int RowIndex, int ColumnIndex) GetReflectionScore(Grid<char> map)
        {
            var columnIndex = FindReflectionColumnIndex(map);

            if (columnIndex > 0)
            {
                return (columnIndex, -2, columnIndex);
            }

            var rowIndex = FindReflectionRowIndex(map);

            if (rowIndex > 0)
            {
                return ((map.Height - rowIndex) * HorizontalScoreFactor, rowIndex, -2);
            }

            throw new Exception($"No reflection row or column found.");
        }

        private static int FindReflectionColumnIndex(Grid<char> map, int excludeColumn = -1)
        {
            for (int i = 1; i < map.Width; i++)
            {
                for (int c = 0; c < (map.Width / 2) + 1; c++)
                {
                    var column1Index = i - 1 - c;
                    var column2Index = i + c;

                    if (column1Index < 0 || column2Index >= map.Width)
                    {
                        if (i == excludeColumn) break;
                        else return i;
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

        private static int FindReflectionRowIndex(Grid<char> map, int excludeRow = -1)
        {
            for (int i = 1; i < map.Height; i++)
            {
                for (int r = 0; r < (map.Height / 2) + 1; r++)
                {
                    var row1Index = i - 1 - r;
                    var row2Index = i + r;

                    if (row1Index < 0 || row2Index >= map.Height)
                    {
                        if (i == excludeRow) break;
                        else return i;
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
