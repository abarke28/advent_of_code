using Aoc.Common;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_12
{
    // https://adventofcode.com/2023/day/12
    public class Day_12 : ISolver
    {
        private class ConditionRecord
        {
            public int[] Nums { get; set; }
            public string Springs { get; set; } = string.Empty;

            public ConditionRecord(string s, IEnumerable<int> nums)
            {
                Nums = nums.ToArray();
                Springs = s;
            }

            public ConditionRecord Expand()
            {
                return new ConditionRecord(
                    string.Join(Unknown, Enumerable.Repeat(this.Springs, ExpansionFactor)) + Operational,
                    Enumerable.Repeat(this.Nums, 5).SelectMany(n => n).ToArray());
            }
        }

        private const char Operational = '.';
        private const char Damaged = '#';
        private const char Unknown = '?';

        private const int ExpansionFactor = 5;

        private static readonly Dictionary<string, long> FunctionCache = new Dictionary<string, long>();

        public object Part1(IList<string> lines)
        {
            var records = ParseRecords(lines);
            records.ForEach(r => r.Springs  += Operational);

            var solutions = records.Select(r => NumSolutions(r.Springs, r.Nums.ToArray(), positionInCurrentGroup: 0));

            var sum = solutions.Sum();

            return sum;   
        }

        public object Part2(IList<string> lines)
        {
            var records = ParseRecords(lines);
            var expandedRecords = records.Select(r => r.Expand()).ToList();

            var solutions = expandedRecords.Select(r => NumSolutions(r.Springs, r.Nums.ToArray(), positionInCurrentGroup: 0));

            var sum = solutions.Sum();

            return sum;
        }

        private static long NumSolutions(string stringLeftToProcess, int[] lengthsLeftToProcess, int positionInCurrentGroup)
        {
            var key = $"{stringLeftToProcess}-{string.Join(',', lengthsLeftToProcess)}-{positionInCurrentGroup}";

            if (FunctionCache.TryGetValue(key, out var cachedResult)) return cachedResult;

            if (string.IsNullOrWhiteSpace(stringLeftToProcess))
            {
                var result = lengthsLeftToProcess.Length == 0 ? 1 : 0;

                FunctionCache.Add(key, result);
                return result;
            }

            var numSolutions = 0L;

            var possibleNextChars = stringLeftToProcess[0] == Unknown ? new char[] { Damaged, Operational } : new char[] { stringLeftToProcess[0] };

            foreach (var possibleChar in possibleNextChars)
            {
                if (possibleChar == Damaged)
                {
                    numSolutions += NumSolutions(stringLeftToProcess[1..], lengthsLeftToProcess, positionInCurrentGroup + 1);
                }
                else
                {
                    if (positionInCurrentGroup > 0)
                    {
                        if (lengthsLeftToProcess.Length > 0 && lengthsLeftToProcess[0] == positionInCurrentGroup)
                        {
                            numSolutions += NumSolutions(stringLeftToProcess[1..], lengthsLeftToProcess[1..], positionInCurrentGroup: 0);
                        }
                    }
                    else
                    {
                        numSolutions += NumSolutions(stringLeftToProcess[1..], lengthsLeftToProcess, positionInCurrentGroup: 0);
                    }
                }
            }

            FunctionCache.Add(key, numSolutions);
            return numSolutions;
        }

        private static List<ConditionRecord> ParseRecords(IList<string> lines)
        {
            var records = new List<ConditionRecord>();

            foreach (var line in lines)
            {
                var nums = line.ReadAllNumbers<int>(new[] { ' ', ',' });
                var springs = line.Split(' ')[0];

                var record = new ConditionRecord(springs, nums);

                records.Add(record);
            }

            return records;
        }
    }
}
