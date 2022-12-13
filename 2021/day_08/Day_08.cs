using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2021.day_08
{
    // https://adventofcode.com/2021/day/08
    public class Day_08 : ISolver
    {
        private class InputOutput
        {
            public List<string> Inputs { get; set; } = new List<string>();
            public List<string> Outputs { get; set; } = new List<string>();

            public static InputOutput FromString(string s)
            {
                var inputOutputs = s.GetWords('|').ToArray();
                var inputs = inputOutputs[0].Trim().GetWords().ToList();
                var outputs = inputOutputs[1].Trim().GetWords().ToList();

                return new InputOutput
                { 
                    Inputs = inputs,
                    Outputs = outputs
                };
            }
        }

        [Flags]
        private enum Segments
        {
            Top,
            TopLeft,
            TopRight,
            Center,
            BottomLeft,
            BottomRight,
            Bottom
        }

        private static readonly Dictionary<int, int> DigitSegmentCounts = new()
        {
            { 0, 6 },
            { 1, 2 }, // Unique
            { 2, 5 },
            { 3, 5 },
            { 4, 4 }, // Unique
            { 5, 5 },
            { 6, 6 },
            { 7, 3 }, // Unique
            { 8, 7 }, // Unique
            { 9, 5 }
        };

        private static readonly Dictionary<int, Segments> DigitSegments = new()
        {
            { 0, Segments.Top & Segments.TopLeft & Segments.TopRight & Segments.BottomLeft & Segments.BottomRight & Segments.Bottom },
            { 1, Segments.TopRight & Segments.BottomRight },
            { 2, Segments.Top & Segments.TopRight & Segments.Center & Segments.BottomLeft & Segments.Bottom },
            { 3, Segments.Top & Segments.TopRight & Segments.Center & Segments.BottomRight & Segments.Bottom },
            { 4, Segments.TopLeft & Segments.TopRight & Segments.Center & Segments.BottomRight },
            { 5, Segments.Top & Segments.TopLeft & Segments.Center & Segments.BottomRight & Segments.Bottom },
            { 6, Segments.Top & Segments.TopLeft & Segments.Center & Segments.BottomRight & Segments.Bottom & Segments.BottomLeft },
            { 7, Segments.Top & Segments.TopRight & Segments.BottomRight },
            { 8, Segments.Top & Segments.TopLeft & Segments.TopRight & Segments.Center & Segments.BottomLeft & Segments.BottomRight & Segments.Bottom },
            { 9, Segments.Top & Segments.TopLeft & Segments.TopRight & Segments.Center & Segments.BottomRight & Segments.Bottom }
        };

        private static readonly Dictionary<Segments, int> Digit = new()
        {
            { Segments.Top & Segments.TopLeft & Segments.TopRight & Segments.BottomLeft & Segments.BottomRight & Segments.Bottom, 0 },
            { Segments.TopRight & Segments.BottomRight, 1 },
            { Segments.Top & Segments.TopRight & Segments.Center & Segments.BottomLeft & Segments.Bottom, 2 },
            { Segments.Top & Segments.TopRight & Segments.Center & Segments.BottomRight & Segments.Bottom , 3},
            { Segments.TopLeft & Segments.TopRight & Segments.Center & Segments.BottomRight, 4 },
            { Segments.Top & Segments.TopLeft & Segments.Center & Segments.BottomRight & Segments.Bottom, 5},
            { Segments.Top & Segments.TopLeft & Segments.Center & Segments.BottomRight & Segments.Bottom & Segments.BottomLeft, 6 },
            { Segments.Top & Segments.TopRight & Segments.BottomRight, 7 },
            { Segments.Top & Segments.TopLeft & Segments.TopRight & Segments.Center & Segments.BottomLeft & Segments.BottomRight & Segments.Bottom, 8 },
            { Segments.Top & Segments.TopLeft & Segments.TopRight & Segments.Center & Segments.BottomRight & Segments.Bottom, 9}
        };

        private static readonly Dictionary<Segments, List<int>> SegmentDigits = new()
        {
            { Segments.Top, new List<int> { 0, 2, 3, 5, 6, 7, 8, 9 } },
            { Segments.TopLeft, new List<int> { 0, 4, 5, 6, 8, 9 } },
            { Segments.TopRight, new List<int> { 0, 1, 2, 3, 4, 7, 8, 9} },
            { Segments.Center, new List<int> { 2, 3, 4, 5, 6, 8, 9 } },
            { Segments.BottomLeft, new List<int> { 0, 2, 6, 8 } },
            { Segments.BottomRight, new List<int> { 0, 1, 3, 4, 5, 6, 7, 8, 9 } },
            { Segments.Bottom, new List<int> { 0, 2, 3, 5, 6, 8, 9 } }
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_08/input.txt");

            Console.WriteLine(CalculateUniqueCharsInAllOutputs(lines));

            var inputOutputs = lines.Select(l => InputOutput.FromString(l)).ToList();

        }

        private static int DecodeOutput(InputOutput entry)
        {
            var inputs = entry.Inputs;
            var outputs = entry.Outputs;

            var one = inputs.Single(i => i.Length == DigitSegmentCounts[1]);
            var four = inputs.Single(i => i.Length == DigitSegmentCounts[4]);
            var seven = inputs.Single(i => i.Length == DigitSegmentCounts[7]);
            var eight = inputs.Single(i => i.Length == DigitSegmentCounts[8]);



            return 0;
        }

        private static int CalculateUniqueCharsInAllOutputs(IEnumerable<string> lines)
        {
            var inputOutputs = lines.Select(l => l.GetWords('|'));
            var inputs = inputOutputs.Select(io => io.ToArray()[0].Trim().GetWords());
            var outputs = inputOutputs.Select(io => io.ToArray()[1].Trim().GetWords());

            var flatOutputs = outputs.SelectMany(o => o);

            var countOfUnique = flatOutputs.Count(o => o.Length == 2 ||
                                                       o.Length == 3 ||
                                                       o.Length == 4 ||
                                                       o.Length == 7);

            return countOfUnique;
        }
    }
}
