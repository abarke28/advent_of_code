using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2021.day_08
{
    // https://adventofcode.com/2021/day/08
    public class Day_08 : ISolver
    {
        private const int SegmentCount069 = 6;
        private const int SegmentCount235 = 5;

        private const int OutputCount = 4;

        [Flags]
        private enum Segment
        {
            None = 0,
            Top = 1,
            TopLeft = 2,
            TopRight = 4,
            Center = 8,
            BottomLeft = 16,
            BottomRight = 32,
            Bottom = 64
        }

        private class InputOutput
        {
            public List<string> Inputs { get; set; } = new List<string>();
            public List<string> Outputs { get; set; } = new List<string>();

            public IEnumerable<string> Set069 => Inputs.Where(l => l.Length == SegmentCount069);
            public IEnumerable<string> Set235 => Inputs.Where(l => l.Length == SegmentCount235);
            public IEnumerable<string> Set1478 => Inputs.Where(l => l.Length != SegmentCount235 && l.Length != SegmentCount069);

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

        private static readonly List<char> Signals = new()
        {
            'a',
            'b',
            'c',
            'd',
            'e',
            'f',
            'g'
        };

        private static readonly Dictionary<Segment, int> SegmentsDigitMap = new()
        {
            { Segment.Top | Segment.TopLeft | Segment.TopRight | Segment.BottomLeft | Segment.BottomRight | Segment.Bottom, 0 },
            { Segment.TopRight | Segment.BottomRight, 1 },
            { Segment.Top | Segment.TopRight | Segment.Center | Segment.BottomLeft | Segment.Bottom, 2 },
            { Segment.Top | Segment.TopRight | Segment.Center | Segment.BottomRight | Segment.Bottom , 3},
            { Segment.TopLeft | Segment.TopRight | Segment.Center | Segment.BottomRight, 4 },
            { Segment.Top | Segment.TopLeft | Segment.Center | Segment.BottomRight | Segment.Bottom, 5},
            { Segment.Top | Segment.TopLeft | Segment.Center | Segment.BottomRight | Segment.Bottom | Segment.BottomLeft, 6 },
            { Segment.Top | Segment.TopRight | Segment.BottomRight, 7 },
            { Segment.Top | Segment.TopLeft | Segment.TopRight | Segment.Center | Segment.BottomLeft | Segment.BottomRight | Segment.Bottom, 8 },
            { Segment.Top | Segment.TopLeft | Segment.TopRight | Segment.Center | Segment.BottomRight | Segment.Bottom, 9}
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_08/input.txt");

            var inputOutputs = lines.Select(l => InputOutput.FromString(l)).ToList();

            var decodedOutputs = inputOutputs.Select(io => GetOutputNums(io));
            var outputNums = decodedOutputs.Select(dos => GetOutputNum(dos.ToList()));

            Console.WriteLine(outputNums.Sum());
        }

        private static int GetOutputNum(IList<int> digits)
        {
            if (digits.Count != OutputCount)
            {
                throw new Exception("Invalid output");
            }

            return int.Parse($"{digits[0]}{digits[1]}{digits[2]}{digits[3]}");
        }

        private static IEnumerable<int> GetOutputNums(InputOutput entry)
        {
            var segmentMap = DecodeSegments(entry);
            var digits = DecodeOutput(entry, segmentMap);

            return digits;
        }

        private static Dictionary<char, Segment> DecodeSegments(InputOutput segmentSignals)
        {
            var charSegmentMap = new Dictionary<char, Segment>();

            foreach (var signal in Signals)
            {
                var segment069Count = segmentSignals.Set069.Count(s => s.Contains(signal));
                var segment235Count = segmentSignals.Set235.Count(s => s.Contains(signal));
                var segment1478Count = segmentSignals.Set1478.Count(s => s.Contains(signal));

                var segment = (segment069Count, segment235Count, segment1478Count) switch
                {
                    (3, 3, 2) => Segment.Top,
                    (3, 1, 2) => Segment.TopLeft,
                    (2, 2, 4) => Segment.TopRight,
                    (2, 3, 2) => Segment.Center,
                    (2, 1, 1) => Segment.BottomLeft,
                    (3, 2, 4) => Segment.BottomRight,
                    (3, 3, 1) => Segment.Bottom,
                    _ => throw new Exception("Unexpected segment pattern.")
                };

                charSegmentMap.Add(signal, segment);
            }

            return charSegmentMap;
        }

        private static IEnumerable<int> DecodeOutput(InputOutput entry, Dictionary<char, Segment> segmentMap)
        {
            var nums = new List<int>(OutputCount);

            foreach (var output in entry.Outputs)
            {
                var segments = Segment.None;

                foreach (var c in output)
                {
                    var newSegment = segmentMap[c];

                    segments |= newSegment;
                }

                var num = SegmentsDigitMap[segments];
                nums.Add(num);
            }

            return nums;
        }
    }
}
