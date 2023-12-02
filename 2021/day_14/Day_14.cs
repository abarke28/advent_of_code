using aoc.common;
using aoc.utils;
using aoc.utils.extensions;
using System.Text;
using System.Text.RegularExpressions;

namespace aoc.y2021.day_14
{
    // https://adventofcode.com/2021/day/14
    public class Day_14
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_14/input.txt");

            var (polymer, rules) = ParseInput(lines);

            var evolvedPolymer = ApplyRulesNTimes(polymer, rules, 10);
            var polymerScore = GetPolymerScore(evolvedPolymer);

            Console.WriteLine(polymerScore);

            //var evolvedPolymer2 = ApplyRulesNTimes(polymer, rules, 40);
            //var polymerScore2 = GetPolymerScore(evolvedPolymer2);

            //Console.WriteLine(polymerScore2);
        }

        private static string ApplyRulesNTimes(string polymer, IList<(Regex Pattern, char Insertion)> rules, int times)
        {
            var initialCount = times;
            var currentPolymer = polymer;
            while (times-- > 0)
            {
                Console.WriteLine($"\nRound #: {initialCount - times}");
                currentPolymer = ApplyRules(currentPolymer, rules);
            }

            return currentPolymer;
        }

        private static string ApplyRules(string polymer, IList<(Regex Pattern, char Insertion)> rules)
        {
            var insertionCharsMap = new List<(int Index, char InsertionChar)>();

            for (int i = 0; i < rules.Count; i++)
            {
                var pattern = rules[i].Pattern;
                var insertionChar = rules[i].Insertion;

                var matches = polymer.AllIndexesOf(pattern);

                foreach (var matchIndex in matches)
                {
                    insertionCharsMap.Add((matchIndex + 1, insertionChar));
                }
            }

            var polymerSb = new StringBuilder(polymer);

            insertionCharsMap.Sort((l, r) => l.Index.CompareTo(r.Index));

            var addedChars = insertionCharsMap.Select(kvp => kvp.InsertionChar).Distinct();
            var insertionCharCount = addedChars.ToDictionary(ac => ac, ac => insertionCharsMap.Where(kvp => kvp.InsertionChar == ac).Count());

            //foreach (var kvp in insertionCharCount)
            //{
            //    Console.WriteLine($"Inserting: {kvp.Key} x {kvp.Value}");
            //}

            var insertionCount = 0;

            foreach (var (insertionIndex, insertionChar) in insertionCharsMap)
            {
                polymerSb.Insert(insertionIndex + insertionCount++, insertionChar);
            }

            var result = polymerSb.ToString();

            Console.WriteLine(GetPolymerScore(result));

            return result;
        }

        private static long GetPolymerScore(string polymer)
        {
            const int alphabetSize  = 26;
            var charCount = new Dictionary<char, long>(alphabetSize);

            foreach (var c in polymer)
            {
                if (charCount.ContainsKey(c))
                {
                    charCount[c]++;
                }
                else
                {
                    charCount.Add(c, 1);
                }
            }

            var charList = charCount.ToList();
            charList.Sort((l, r) => l.Value.CompareTo(r.Value));

            return charList.Last().Value - charList.First().Value;
        }

        private static (string Polymer, IList<(Regex Pattern, char Insertion)> rules) ParseInput(IEnumerable<string> lines)
        {
            var polymer = lines.First().Trim();

            var rules = lines.Skip(2)
                             .Select(l => l.Split(" -> ").ToList())
                             .Select(r => (Pattern: new Regex($"(?=({r[0]}))", RegexOptions.Compiled), Insertion: r[1].Single()))
                             .ToList();

            return (polymer, rules);
        }
    }
}
