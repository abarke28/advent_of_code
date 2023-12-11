using Aoc.Common;
using Aoc.Utils;

namespace Aoc.y2022.day_04
{
    // https://adventofcode.com/2022/day/4
    public class Day_04
    {
        private class CleaningAssignment
        {
            public int Start { get; set; }
            public int End { get; set; }
        }

        private class ElfPair
        {
            public CleaningAssignment Elf1 { get; set; } = new CleaningAssignment();
            public CleaningAssignment Elf2 { get; set; } = new CleaningAssignment();

            public bool AssignmentsTotallyOverlap()
            {
                return
                    (Elf1.Start <= Elf2.Start && Elf1.End >= Elf2.End) ||
                    (Elf2.Start <= Elf1.Start && Elf2.End >= Elf1.End);
            }

            public bool AssignmentsOverlapAtAll()
            {
                return
                    (Elf1.Start <= Elf2.Start && Elf1.End >= Elf2.Start) ||
                    (Elf1.Start <= Elf2.End && Elf1.End >= Elf2.End) ||
                    (Elf2.Start <= Elf1.Start && Elf2.End >= Elf1.Start) ||
                    (Elf1.Start <= Elf2.End && Elf1.End >= Elf2.End);
            }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2022/day_04/input.txt");

            var elfPairs = lines.Select(l => ParseCleaningAssignments(l));

            var totalOverlappers = elfPairs.Where(ep => ep.AssignmentsTotallyOverlap());
            var partialOverlappers = elfPairs.Where(ep => ep.AssignmentsOverlapAtAll());

            Console.WriteLine(totalOverlappers.Count());
            Console.WriteLine(partialOverlappers.Count());
        }

        private static ElfPair ParseCleaningAssignments(string s)
        {
            var assignments = s.Split(',');

            if (assignments.Length != 2)
            {
                throw new Exception("Failed to parse assignment pair");
            }

            var firstAssignment = assignments[0].Split('-');

            if (firstAssignment.Length != 2)
            {
                throw new Exception("Failed to parse first assignment");
            }

            var elf1 = new CleaningAssignment
            {
                Start = int.Parse(firstAssignment[0]),
                End = int.Parse(firstAssignment[1])
            };

            var secondAssignment = assignments[1].Split('-');

            if (secondAssignment.Length != 2)
            {
                throw new Exception("Failed to parse second assignment");
            }

            var elf2 = new CleaningAssignment
            {
                Start = int.Parse(secondAssignment[0]),
                End = int.Parse(secondAssignment[1])
            };

            return new ElfPair
            {
                Elf1 = elf1,
                Elf2 = elf2
            };
        }
    }
}
