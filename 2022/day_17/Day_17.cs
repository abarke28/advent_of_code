using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2022.day_17
{
    // https://adventofcode.com/2022/day/17
    public class Day_17
    {
        private const int ChamberWidth = 7;
        private const int BottomOffset = 4;
        private const int LeftOffset = 2;
        private const int RockCycleCount = 5;
        private const int MaxShapeRockCount = 5;

        private static readonly Dictionary<char, Vector2D> GasVectorMap = new()
        {
            { '<', Vector2D.Left },
            { '>', Vector2D.Right },
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_17/input.txt");

            var gasVectors = GetGasPushes(lines.Single());

            var fallenRocks = new HashSet<Vector2D>()
            {
                new Vector2D(0, -1)
            };

            var numRocks = 2022;
            var deltas = SimulateNRockFalls(numRocks, fallenRocks, gasVectors);

            var height = fallenRocks.Max(fr => fr.Y) + 1;
            Console.WriteLine($"Height after {numRocks} rocks: {height}\n");

            var (cycleCount, cycleSize, cycleRemainder) = FindCycle(deltas);

            gasVectors = GetGasPushes(lines.Single());

            fallenRocks = new HashSet<Vector2D>()
            {
                new Vector2D(0, -1)
            };

            SimulateNRockFalls(cycleRemainder, fallenRocks, gasVectors);

            var baseHeight = fallenRocks.Max(fr => fr.Y) + 1;
            Console.WriteLine($"Height after 1 trillion rocks: {baseHeight + (cycleCount * cycleSize)}");
        }

        private static (long CycleLength, long CycleSize, long CycleRemainder) FindCycle(IList<int> deltas)
        {
            var desiredRocks = 1_000_000_000_000;
            var skipFirstCount = 250;

            deltas = deltas.Skip(skipFirstCount).ToList();
            var cycleLength = 0;

            for (var candidateCycleLength = 1; candidateCycleLength < deltas.Count; candidateCycleLength++)
            {
                var cycleLengthFound = true;

                for (var i = 0; i < deltas.Count - candidateCycleLength; i++)
                {
                    if (deltas[i] != deltas[i + candidateCycleLength])
                    {
                        cycleLengthFound = false;
                        break;
                    }
                }

                if (cycleLengthFound)
                {
                    cycleLength = candidateCycleLength;
                    break;
                }
            }

            var cycleSize = (long)deltas.Take(cycleLength).Aggregate((l, r) => l + r);

            var cycleCount = desiredRocks / cycleLength;
            var cycleRem = desiredRocks % cycleLength;

            return (cycleCount, cycleSize, cycleRem);
        }

        private static IList<int> SimulateNRockFalls(long numOfRocks, HashSet<Vector2D> fallenRocks, Queue<Vector2D> gasVectors)
        {
            var deltas = new List<int>();

            for (var i = 0; i < numOfRocks; i++)
            {
                SimulateRockFall(i, fallenRocks, gasVectors, deltas);

                // Manage vector set size, only top few actually matter for interactions.
                if (i % 100 == 0)
                {
                    fallenRocks.RemoveWhere(fr => fr.Y < (fallenRocks.Max(fr => fr.Y) - 100));
                }
            }

            return deltas;
        }

        private static void SimulateRockFall(long rockNum, HashSet<Vector2D> fallenRocks, Queue<Vector2D> gasVectors, IList<int> deltas)
        {
            var maxY = fallenRocks.Max(fr => fr.Y);
            var newRockY = maxY + BottomOffset;

            var newRock = GetRockN(rockNum, newRockY);

            var rockIsAtRest = false;

            while (!rockIsAtRest)
            {
                var gasAffect = gasVectors.Dequeue();
                gasVectors.Enqueue(gasAffect);
                newRock = SimulateGasAffect(newRock, gasAffect, fallenRocks);
                (newRock, rockIsAtRest) = SimulateFall(newRock, fallenRocks);
            }

            fallenRocks.AddRange(newRock);

            var heightDelta = newRock.Max(nr => nr.Y) - maxY;
            deltas.Add(Math.Max(heightDelta, 0));
        }

        private static (IEnumerable<Vector2D> Position, bool AtRest) SimulateFall(IEnumerable<Vector2D> rockPosition, HashSet<Vector2D> currentRocks)
        {
            var newRockPosition = rockPosition.Select(rp => rp + Vector2D.Down);

            var moveNotAllowed = newRockPosition.Any(nrp => nrp.Y < 0) ||
                                 newRockPosition.Any(nrp => currentRocks.Contains(nrp));

            return moveNotAllowed ? (rockPosition, AtRest: true) : (newRockPosition, AtRest: false);
        }

        private static IEnumerable<Vector2D> SimulateGasAffect(IEnumerable<Vector2D> rockPosition, Vector2D gasVector, HashSet<Vector2D> currentRocks)
        {
            var newRockPosition = rockPosition.Select(rp => rp + gasVector);

            var moveNotAllowed = newRockPosition.Any(trp => trp.X < 0 || trp.X >= ChamberWidth) ||
                                 newRockPosition.Any(trp => currentRocks.Contains(trp));

            return moveNotAllowed ? rockPosition : newRockPosition;
        }

        private static IEnumerable<Vector2D> GetRockN(long n, int lowestYCoordinate)
        {
            var sequenceNum = n % RockCycleCount;

            var rockVectors = new List<Vector2D>(MaxShapeRockCount);

            switch (sequenceNum)
            {
                case 0:
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 1, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 2, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 3, lowestYCoordinate));
                    return rockVectors;
                case 1:
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate + 1));
                    rockVectors.Add(new Vector2D(LeftOffset + 1, lowestYCoordinate + 1));
                    rockVectors.Add(new Vector2D(LeftOffset + 2, lowestYCoordinate + 1));
                    rockVectors.Add(new Vector2D(LeftOffset + 1, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 1, lowestYCoordinate + 2));
                    return rockVectors;
                case 2:
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 1, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 2, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 2, lowestYCoordinate + 1));
                    rockVectors.Add(new Vector2D(LeftOffset + 2, lowestYCoordinate + 2));
                    return rockVectors;
                case 3:
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate + 1));
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate + 2));
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate + 3));
                    return rockVectors;
                case 4:
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset + 1, lowestYCoordinate));
                    rockVectors.Add(new Vector2D(LeftOffset, lowestYCoordinate + 1));
                    rockVectors.Add(new Vector2D(LeftOffset + 1, lowestYCoordinate + 1));
                    return rockVectors;
                default:
                    throw new Exception("Invalid rock sequence num");

            }
        }

        private static Queue<Vector2D> GetGasPushes(string s)
        {
            var gasVectors = new Queue<Vector2D>(s.Length);

            foreach (var c in s)
            {
                gasVectors.Enqueue(GasVectorMap[c]);
            }

            return gasVectors;
        }
    }
}
