using aoc.common;
using aoc.utils.extensions;

namespace aoc.y2023.day_05
{
    // https://adventofcode.com/2023/day/05
    public class Day_05 : ISolver
    {
        private class FarmMap
        {
            public long DestinationStart { get; set; }
            public long SourceStart { get; set; }
            public long RangeLength { get; set; }

            public static FarmMap FromString(string s)
            {
                var nums = s.ReadAllNumbersLong().ToArray();

                return new FarmMap
                {
                    DestinationStart = nums[DestinationIndex],
                    SourceStart = nums[SourceIndex],
                    RangeLength = nums[RangeLengthIndex]
                };
            }

            public bool MapCoversInput(long input)
            {
                return SourceStart <= input && (SourceStart + RangeLength - 1) >= input;
            }

            public long Map(long input)
            {
                return DestinationStart + (input - SourceStart);
            }
        }

        private const int DestinationIndex = 0;
        private const int SourceIndex = 1;
        private const int RangeLengthIndex = 2;

        public object Part1(IList<string> lines)
        {
            var seeds = lines[0].ReadAllNumbersLong();

            var farmMapLists = ParseMaps(lines.Skip(2));

            var locations = seeds.Select(s => FindSeedLocation(s, farmMapLists));

            return locations.Min();
        }

        public object Part2(IList<string> lines)
        {
            var seedRanges = lines[0].ReadAllNumbersLong().Chunk(2);
            var reverseLists = ParseReverseMaps(lines.Skip(2));

            var candidateLocation = 0;
            var foundSeed = false;

            while (!foundSeed)
            {
                candidateLocation++;

                var candidateLocationSeed = FindSeedLocation(candidateLocation, reverseLists);

                foreach (var seedRange in seedRanges)
                {
                    if (seedRange[0] <= candidateLocationSeed && candidateLocationSeed < (seedRange[0] + seedRange[1]))
                    {
                        foundSeed = true;
                        break;
                    }
                }
            }

            return candidateLocation;
        }

        private long FindSeedLocation(long seedNum, List<List<FarmMap>> farmMapLists)
        {
            var currentNum = seedNum;

            foreach (var farmMapList in farmMapLists)
            {
                var matchingEntry = farmMapList.Where(fml => fml.MapCoversInput(currentNum));

                if (matchingEntry.Any())
                {
                    currentNum = matchingEntry.First().Map(currentNum);
                }
            }

            return currentNum;
        }

        private List<List<FarmMap>> ParseMaps(IEnumerable<string> lines)
        {
            var numsAndBlanks = lines.Where(l => !l.Contains(':'));

            var chunks = new List<List<string>>();
            for (int i = 0; i < numsAndBlanks.Count();)
            {
                var chunk = numsAndBlanks.Skip(i).TakeWhile(l => !string.IsNullOrWhiteSpace(l)).ToList();
                chunks.Add(chunk);

                i += chunk.Count;
                i++;
            }

            var result = chunks.Select(c => c.Select(l => FarmMap.FromString(l)).ToList()).ToList();

            return result;
        }

        private List<List<FarmMap>> ParseReverseMaps(IEnumerable<string> lines)
        {
            var reverseLines = lines.Reverse();

            var numsAndBlanks = reverseLines.Where(l => !l.Contains(':'));

            var chunks = new List<List<string>>();
            for (int i = 0; i < numsAndBlanks.Count();)
            {
                var chunk = numsAndBlanks.Skip(i).TakeWhile(l => !string.IsNullOrWhiteSpace(l)).ToList();
                chunks.Add(chunk);

                i += chunk.Count;
                i++;
            }

            var result = chunks
                .Select(c => c.Select(l => l.ReadAllNumbersLong().ToArray())
                .Select(nums => new FarmMap { RangeLength = nums[RangeLengthIndex], SourceStart = nums[DestinationIndex], DestinationStart = nums[SourceIndex] }).ToList())
                .ToList();

            return result;
        }
    }
}
