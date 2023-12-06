using aoc.common;
using aoc.utils.extensions;

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

            var result = waysToWin.Select(w => w.Count).Aggregate(1, (r1, r2) => r1 * r2);

            var numWaysToWin = races.Select(r => GetNumWaysToWin(r));
            return numWaysToWin.Product();
        }

        public object Part2(IList<string> lines)
        {
            var t = long.Parse(new string(lines[0].Where(c => c.IsNumber()).ToArray()));
            var r = long.Parse(new string(lines[1].Where(c => c.IsNumber()).ToArray()));

            return GetNumWaysToWin(new Race(t, r));
        }

        private static int GetNumWaysToWin(Race race)
        {
            /*
             * speed       = wait
             * travelTime  = time - wait
             * distance    = speed * travelTime = (wait) * (time - wait)
             * distance    = wt - w^2
             * 
             * Therefore...
             * r < wt - w^2
             * 0 < w^2 - tw + r
             * 
             * a = 1, b = -t, c = r
             */
            var a = 1;
            var b = -1 * race.Time;
            var c = race.Record;

            var preRoot = (b * b) - (4 * a * c);

            var root1 = ((-1.0 * b) - Math.Sqrt(preRoot)) / (2.0 * a);
            var root2 = ((-1.0 * b) + Math.Sqrt(preRoot)) / (2.0 * a);

            // If roots are integers, we must exclude them since we need to beat the distance, not match it.
            if ((int)root1 == root1) root1++;
            if ((int)root2 == root2) root2--;

            // Need to add one and floor to get integers between roots. E.g: 1.9 & 2.1 has (1) integer between them.
            // But 2.1 - 1.9 = 0.2. 
            var numWaysToWin = Math.Floor(Math.Floor(root2) - Math.Ceiling(root1) + 1);

            return (int)numWaysToWin;
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
