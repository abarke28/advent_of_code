using aoc.common;

namespace aoc.y2022.day_03
{
    // https://adventofcode.com/2022/day/3
    public class Day_03
    {
        private class Bag
        {
            public string FirstCompartment { get; set; } = string.Empty;
            public string SecondCompartment { get; set; } = string.Empty;
        }

        private class ElfGroup
        {
            public string FirstBag { get; set; } = string.Empty;
            public string SecondBag { get; set; } = string.Empty;
            public string ThirdBag { get; set; } = string.Empty;
        }

        public void Solve()
        {
            var bags = GetBags();

            var scores = bags.Select(b => GetBagDuplicatePriorityScore(b));

            Console.WriteLine(scores.Sum());

            var groups = GetElfGroups();

            var groupScores = groups.Select(g => GetElfGroupDuplicatePriorityScore(g));

            Console.WriteLine(groupScores.Sum()); ;
        }

        private static List<Bag> GetBags()
        {
            var lines = GetLines();

            var bags = lines.Select(l => new Bag
            {
                FirstCompartment = l.Substring(0, l.Length / 2),
                SecondCompartment = l.Substring(l.Length / 2)
            });

            return bags.ToList();
        }

        private static IEnumerable<ElfGroup> GetElfGroups()
        {
            var lines = GetLines();

            for (int i = 0; i < lines.Count(); i += 3)
            {
                yield return new ElfGroup
                {
                    FirstBag = lines[i],
                    SecondBag = lines[i + 1],
                    ThirdBag = lines[i + 2]
                };
            }
        }

        private static int GetBagDuplicatePriorityScore(Bag bag)
        {
            HashSet<char> encounteredItems = bag.FirstCompartment.ToHashSet();
            HashSet<char> duplicateItems = new HashSet<char>();

            foreach (var c in bag.SecondCompartment)
            {
                if (encounteredItems.Contains(c))
                {
                    duplicateItems.Add(c);
                }
            }

            var score = duplicateItems.Sum(c => GetLetterScore(c));

            return score;
        }

        private static int GetElfGroupDuplicatePriorityScore(ElfGroup group)
        {
            var firstSetItems = group.FirstBag.ToHashSet<char>();

            foreach(var c in group.SecondBag)
            {
                if (!firstSetItems.Contains(c))
                {
                    continue;
                }

                if (group.ThirdBag.Contains(c))
                {
                    return GetLetterScore(c);
                }
            }

            throw new Exception("No duplicate found");
        }

        private static int GetLetterScore(char c)
        {
            const int asciiLowerCaseFirstCodePoint = 97;

            const int lowerCaseOffset = 96;
            const int upperCaseOffset = 38;


            var asciiCode = (int)c;

            var priorityCode = asciiCode >= asciiLowerCaseFirstCodePoint
                ? asciiCode - lowerCaseOffset
                : asciiCode - upperCaseOffset;

            return priorityCode;
        }

        private static string[] GetLines()
        {
            return File.ReadAllLines("2022/day_03/input.txt");
        }
    }
}
