using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_10
{
    // https://adventofcode.com/2022/day/10
    public class Day_10
    {
        private class Instruction
        {
            private const string NoOp = "noop";

            private const int AddXCycleCount = 2;
            private const int NoOpCycleCount = 1;

            public int RegisterDelta { get; set; }
            public int CycleCount { get; set; }

            public static Instruction FromString(string s)
            {
                var words = s.GetWords().ToList();
                var isNoOp = words[0] == NoOp;

                return new Instruction
                {
                    CycleCount = isNoOp ? NoOpCycleCount : AddXCycleCount,
                    RegisterDelta = isNoOp ? 0 : int.Parse(words[1])
                };
            }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_10/input.txt");

            var instructions = lines.Select(l => Instruction.FromString(l)).ToList();

            var regValues = GetCycleRegisterValues(instructions);

            var cycles = new List<int> { 20, 60, 100, 140, 180, 220 };

            var result = ComputeCyclesScoreSum(regValues, cycles);

            Console.WriteLine($"\n\nCycle score sum is equal to: '{result}'");
        }

        private static List<int> GetCycleRegisterValues(IEnumerable<Instruction> instructions)
        {
            var registerValues = new List<int>();
            var currentRegisterValue = 1;
            int totalCycleCount = 0;

            foreach (var instruction in instructions)
            {
                while (instruction.CycleCount-- > 0)
                {
                    WriteSpriteToConsole(currentRegisterValue, totalCycleCount);
                    registerValues.Add(currentRegisterValue);
                    totalCycleCount++;
                }

                currentRegisterValue += instruction.RegisterDelta;
            }

            return registerValues;
        }

        private static int ComputeCyclesScoreSum(List<int> registerValues, List<int> cycles)
        {
            var score = 0;

            foreach (var cycle in cycles)
            {
                score += cycle * registerValues[cycle - 1];
            }

            return score;
        }

        private static void WriteSpriteToConsole(int registerValue, int cycleCount)
        {
            const int PixelWidth = 40;

            const char Lit = '#';
            const char NoLit = '.';

            var registerPixels = new int[] { registerValue - 1, registerValue, registerValue + 1 };

            if (cycleCount % PixelWidth == 0)
            {
                Console.WriteLine();
            }

            var pixelIsVisible = registerPixels.Contains(cycleCount % PixelWidth);

            Console.Write(pixelIsVisible ? Lit : NoLit);
        }
    }
}
