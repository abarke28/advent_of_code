using Aoc.Common;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_15
{
    // https://adventofcode.com/2023/day/15
    public class Day_15 : ISolver
    {
        private record Step (string Label, char Operation, int FocalLength);

        private class Box
        {
            public int BoxNumber { get; set; }
            public List<(string Label, int FocalLength)> Lenses { get; set; } = new List<(string Label, int FocalLength)>();

            public int GetScore()
            {
                var score = Lenses.WithIndex(1).Select(l => l.Item.FocalLength * l.Index * (BoxNumber + 1)).Sum();

                return score;
            }
        }
            
        private const int Multiplier = 17;
        private const int Remainder = 256;

        private const char Remove = '-';
        private const char Add = '=';

        public object Part1(IList<string> lines)
        {
            var strings = ParseLines(lines);

            var hashSum = 0L;

            foreach (var s in strings)
            {
                hashSum += GetHashOutput(s);
            }

            return hashSum;
        }

        public object Part2(IList<string> lines)
        {
            var boxes = Enumerable.Range(0, 256)
                .Select(i => new Box { BoxNumber = i, Lenses = new List<(string Label, int FocalLength)>() })
                .ToList();

            var steps = ParseSteps(lines);

            foreach (var step in steps)
            {
                var boxToOperateOn = GetHashOutput(step.Label);
                var box = boxes[boxToOperateOn];

                if (step.Operation == Add)
                {
                    var lensReplaced = false;

                    for (int i = 0; i < box.Lenses.Count; i++)
                    {
                        if (box.Lenses[i].Item1 == step.Label)
                        {
                            box.Lenses[i] = new (step.Label, step.FocalLength);
                            lensReplaced = true;
                            break;
                        }
                    }

                    if (!lensReplaced)
                    {
                        box.Lenses.Add(new(step.Label, step.FocalLength));
                    }
                }
                else if (step.Operation == Remove)
                {
                    for (int i = 0; i < box.Lenses.Count; i++)
                    {
                        if (box.Lenses[i].Label == step.Label)
                        {
                            box.Lenses.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            var scores = boxes.Select(b => b.GetScore());

            return scores.Sum();
        }

        private static int GetHashOutput(string s) {

            var hash = 0;

            foreach (var c in s)
            {
                hash += c;
                hash *= Multiplier;
                hash %= Remainder;
            }

            return hash;
        }

        private static IEnumerable<string> ParseLines(IList<string> lines)
        {
            return lines.First().Split(',');
        }

        private static IEnumerable<Step> ParseSteps(IList<string> lines)
        {
            var steps = lines.First().Split(',');

            foreach (var step in steps)
            {
                var isAdd = step.Contains(Add);

                var splitstring = step.Split(Add, Remove);

                yield return new Step(splitstring[0], isAdd ? Add : Remove, isAdd ? int.Parse(splitstring[1]) : 0);
            }
        }
    }
}
