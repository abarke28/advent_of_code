using aoc.common;
using aoc.utils;

namespace aoc.y2023.day_08
{
    // https://adventofcode.com/2023/day/08
    public class Day_08 : ISolver
    {
        private record Entry(string Node, string Left, string Right);

        private const char Left = 'L';

        public object Part1(IList<string> lines)
        {
            var instructions = lines[0];

            var entries = ParseEntries(lines.Skip(2));

            var mappedEntries = entries.ToDictionary(e => e.Node, e => (Left: e.Left, Right: e.Right));

            var steps = CountSteps("AAA", "ZZZ", instructions, mappedEntries);

            return steps;
        }

        public object Part2(IList<string> lines)
        {
            var instructions = lines[0];

            var entries = ParseEntries(lines.Skip(2));

            var mappedEntries = entries.ToDictionary(e => e.Node, e => (e.Left, e.Right));

            var steps = CountSteps2(instructions, mappedEntries);

            return steps;
        }

        private static int CountSteps(string startNode, string endNode, string instructions, Dictionary<string, (string Left, string Right)> entries)
        {
            var current = startNode;
            var i = 0;

            while (current != endNode)
            {
                var nextInstruction = instructions[i++ % instructions.Length];
                current = nextInstruction == Left ? entries[current].Left : entries[current].Right;
            }

            return i;
        }

        private static long CountSteps2(string instructions, Dictionary<string, (string Left, string Right)> entries)
        {
            var startingNodes = entries.Keys.Where(n => n.EndsWith('A')).ToHashSet();

            var nodePaths = new List<int>();

            foreach (var startingNode in startingNodes)
            {
                var current = startingNode;
                var i = 0;

                while (!current.EndsWith('Z'))
                {
                    var nextInstruction = instructions[i++ % instructions.Length];
                    current = nextInstruction == Left ? entries[current].Left : entries[current].Right;
                }

                nodePaths.Add(i);
            }

            return MathUtils.LCM(nodePaths.Select(n => (long)n).ToArray());
        }

        private static List<Entry> ParseEntries(IEnumerable<string> lines)
        {
            return lines
                .Select(l => l.Split(new char[] { '=', '(', ',', ')' }, StringSplitOptions.TrimEntries)
                              .Where(s => !string.IsNullOrWhiteSpace(s))
                              .ToArray())
                .Select(l => new Entry(l[0], l[1], l[2]))
                .ToList();
        }
    }
}
