using aoc.common;
using aoc.utils.extensions;
using System.Collections.Generic;

namespace aoc.y2023.day_06
{
    // https://adventofcode.com/2023/day/06
    public class Day_06 : ISolver
    {
        public record Race(long Time, long Record);

        public object Part1(IList<string> lines)
        {
            var races = ParseRaces(lines);

            var waysToWin = races.Select(r => GetWaysToWin(r)).ToList();

            return waysToWin.Select(w => w.Count).Aggregate(1, (r1, r2) => r1 * r2);
        }

        public object Part2(IList<string> lines)
        {
            var t = long.Parse(new string(lines[0].Where(c => c.IsNumber()).ToArray()));
            var r = long.Parse(new string(lines[1].Where(c => c.IsNumber()).ToArray()));

            return GetWaysToWin(new Race(t, r)).Count;
        }

        private static List<(long TimeHeld, long Distance)> GetWaysToWin(Race race)
        {
            var waysToWin = new List<(long TimeHeld, long Distance)>();

            for (long i = 0; i < race.Time; i++)
            {
                var distance = i * (race.Time - i);

                if (distance > race.Record)
                {
                    waysToWin.Add((i, distance));
                }
            }

            return waysToWin;
        }

        private static List<Race> ParseRaces(IList<string> lines)
        {
            var times = lines[0].ReadAllNumbersLong().ToArray();
            var records = lines[1].ReadAllNumbersLong().ToArray();

            var races = times.Zip(records, (t, r) => new Race(t, r));

            return races.ToList();
        }
    }
}
