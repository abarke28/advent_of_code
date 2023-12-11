using Aoc.Common;
using Aoc.Utils;

namespace Aoc.y2021.day_11
{
    // https://adventofcode.com/2021/day/11
    public class Day_11
    {
        private const int MinEnergy = 0;
        private const int MaxEnergy = 9;

        private const int MinCoord = 0;
        private const int MaxCoord = 9;
        
        private const int MaxNeighbors = 8;

        private const int OctopusCount = 100;

        private class Ocotopus
        {
            public int EnergyLevel { get; set; }
            public HashSet<int> RoundsFlashed { get; set; } = new();
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_11/input.txt");

            var octopuses = GetOctopuses(lines);

            var flashes = SimulateNRounds(octopuses, 100);
            Console.WriteLine(flashes);

            var octopuses2 = GetOctopuses(lines);

            var roundNum = GetOctopusSyncRound(octopuses2);
            Console.WriteLine(roundNum);
        }

        private static int GetOctopusSyncRound(IDictionary<Vector2D, Ocotopus> octopuses)
        {
            for (var i = 1; ; i++)
            {
                var totalFlashes = SimulateRound(octopuses, i);

                if (totalFlashes == OctopusCount)
                {
                    return i;
                }
            }

            throw new Exception("No synchronization!");
        }

        private static int SimulateNRounds(IDictionary<Vector2D, Ocotopus> octopuses, int numOfRounds)
        {
            var totalFlashes = 0;

            for (var i = 0; i < numOfRounds; i++)
            {
                totalFlashes += SimulateRound(octopuses, i);
            }

            return totalFlashes;
        }

        private static int SimulateRound(IDictionary<Vector2D, Ocotopus> octopuses, int roundNum)
        {
            foreach (var octopus in octopuses)
            {
                octopus.Value.EnergyLevel += 1;

                if (octopus.Value.EnergyLevel > MaxEnergy && !octopus.Value.RoundsFlashed.Contains(roundNum))
                {
                    SimulateOctopusFlash(octopus.Key, octopuses, roundNum);
                }
            }

            var flashesThisRound = 0;

            foreach (var octopus in octopuses)
            {
                if (octopus.Value.EnergyLevel > MaxEnergy)
                {
                    octopus.Value.EnergyLevel = MinEnergy;
                    flashesThisRound++;
                }
            }

            return flashesThisRound;
        }

        private static void SimulateOctopusFlash(Vector2D octopusPosition, IDictionary<Vector2D, Ocotopus> octopuses, int roundNum)
        {
            var octopus = octopuses[octopusPosition];
            octopus.RoundsFlashed.Add(roundNum);

            var neighborPositions = GetOctopusNeighbors(octopusPosition);

            foreach (var neighborPosition in neighborPositions)
            {
                var neighborOctopus = octopuses[neighborPosition];

                neighborOctopus.EnergyLevel += 1;

                if (neighborOctopus.EnergyLevel > MaxEnergy && !neighborOctopus.RoundsFlashed.Contains(roundNum))
                {
                    SimulateOctopusFlash(neighborPosition, octopuses, roundNum);
                }
            }
        }

        private static IDictionary<Vector2D, Ocotopus> GetOctopuses(IList<string> lines)
        {
            var octopuses = new Dictionary<Vector2D, Ocotopus>();

            for (var i = 0; i <= MaxCoord; i++)
            {
                for (var j = 0; j <= MaxCoord; j++)
                {
                    var octopus = new Ocotopus
                    {
                        EnergyLevel = lines[i][j] - '0'
                    };

                    octopuses.Add(new Vector2D(j, i), octopus);
                }
            }

            return octopuses;
        }

        private static ISet<Vector2D> GetOctopusNeighbors(Vector2D v)
        {
            var neighbors = new HashSet<Vector2D>(MaxNeighbors);

            foreach (var dx in new int[] { -1, 0, 1 })
            {
                foreach (var dy in new int[] { -1, 0 , 1 })
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    var neighbor = new Vector2D(Math.Clamp(v.X + dx, MinCoord, MaxCoord),
                                                Math.Clamp(v.Y + dy, MinCoord, MaxCoord));

                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
    }
}
