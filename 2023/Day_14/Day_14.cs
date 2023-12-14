using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.Y2023.Day_14
{
    // https://adventofcode.com/2023/day/14
    public class Day_14 : ISolver
    {
        private const char Rock = 'O';
        private const char Ground = '.';

        private const int SpinCycles = 1_000_000_000;

        public object Part1(IList<string> lines)
        {
            var map = ParseLines(lines);

            TiltMap(map, Vector2D.Up);

            var score = map.FindAll(c => c == Rock).Sum(r => r.Y + 1);

            return score;
        }

        public object Part2(IList<string> lines)
        {
            var map = ParseLines(lines);
            var mapCopy = map.Copy();

            var tiltCycle = new Vector2D[] { Vector2D.Up, Vector2D.Left, Vector2D.Down, Vector2D.Right };

            var (cycleLeadIn, cycleLength) = FindTiltCycle(map, tiltCycle);

            var rocksAfterNCycles = SimulateRocksAfterNTiltCycles(mapCopy, tiltCycle, cycleLeadIn, cycleLength, SpinCycles);

            var score = rocksAfterNCycles.Sum(r => r.Y + 1);

            return score;
        }

        private static List<Vector2D> SimulateRocksAfterNTiltCycles(Grid<char> map, Vector2D[] tiltCycle, int cycleLeadIn, int cycleLength, int numSequences)
        {
            var remainingTilts = numSequences - cycleLeadIn;
            var extraTiltsToRun = remainingTilts % cycleLength;

            var tiltCyclesToSimulate = cycleLeadIn + extraTiltsToRun;

            for (int i = 0; i < tiltCyclesToSimulate; i++)
            {
                foreach (var tilt in tiltCycle)
                {
                    TiltMap(map, tilt);
                }
            }

            var rocks = map.FindAll(c => c == Rock);

            return rocks.ToList();
        }

        private static (int CycleLeadIn, int CycleLength)  FindTiltCycle(Grid<char> map, Vector2D[] tiltSequence)
        {
            var stateDictionary = new Dictionary<string, int>();
            var i = 0;

            var state = string.Join('-', map.FindAll(c => c == Rock));

            stateDictionary.Add(state, i);

            while (true)
            {
                foreach (var tilt in tiltSequence)
                {
                    TiltMap(map, tilt);
                }

                i++;

                state = string.Join('-', map.FindAll(c => c == Rock));

                if (stateDictionary.ContainsKey(state))
                {
                    var leadIn = stateDictionary[state];

                    var cycleLength = i - leadIn;

                    return (leadIn, cycleLength);
                }
                else
                {
                    stateDictionary.Add(state, i);
                }
            }
        }

        private static void TiltMap(Grid<char> map, Vector2D tiltDirection)
        {
            var rocks = map
                .FindAll(c => c == Rock);

            switch ((tiltDirection.X, tiltDirection.Y))
            {
                case (0, 1):
                    rocks = rocks.OrderByDescending(r => r.Y);
                    break;
                case (-1, 0):
                    rocks = rocks.OrderBy(r => r.X);
                    break;
                case (0, -1):
                    rocks = rocks.OrderBy(r => r.Y);
                    break;
                case (1, 0):
                    rocks = rocks.OrderByDescending(r => r.X);
                    break;
                default:
                    throw new Exception($"Unexpected tilt direction: {tiltDirection}");

            }
               
            foreach (var rock in rocks)
            {
                var rockLocation = rock;
                var nextSlideLocation = rockLocation + tiltDirection;

                while (map.IsInBounds(nextSlideLocation) && map.GetValue(rockLocation + tiltDirection) == Ground)
                {
                    map.SetValue(rockLocation, Ground);
                    map.SetValue(nextSlideLocation, Rock);

                    rockLocation += tiltDirection;
                    nextSlideLocation += tiltDirection;
                }
            }
        }

        private static Grid<char> ParseLines(IList<string> lines)
        {
            return Grid<char>.FromStrings(lines, c => c);
        }
    }
}
