using aoc.common;
using aoc.utils;

namespace aoc.y2021.day_10
{
    // https://adventofcode.com/2021/day/10
    public class Day_10 : ISolver
    {
        private const long AutoCompleteFactor = 5L;

        private static readonly HashSet<char> OpeningChars = new() { '(', '[', '{', '<' };

        private static readonly Dictionary<char, int> CharScoreMap = new()
        {
            { ')', 3 },
            { ']', 57 },
            { '}', 1197 },
            { '>', 25137 }
        };

        private static readonly Dictionary<char, long> CharAutoCompleteScoreMap = new()
        {
            { ')', 1L },
            { ']', 2L },
            { '}', 3L },
            { '>', 4L }
        };

        private static readonly Dictionary<char, char> OpenCloseMap = new()
        {
            { '(', ')' },
            { '[', ']' },
            { '{', '}' },
            { '<', '>' }
        };

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2021/day_10/input.txt");

            var score = 0;
            var incompleteLines = new List<string>(lines.Count);

            foreach (var line in lines)
            {
                if (IsLineCorrupted(line, out var invalidChar))
                {
                    score += CharScoreMap[invalidChar];
                    continue;
                }

                incompleteLines.Add(line);
            }

            Console.WriteLine(score);

            var completionScores = incompleteLines.Select(l => ComputeAutocompleteScore(l)).ToList();
            completionScores.Sort();

            Console.WriteLine(completionScores[completionScores.Count/2]);
        }

        private static bool IsLineCorrupted(string l, out char invalidChar)
        {
            var chunkStack = new Stack<char>();

            foreach (var c in l)
            {
                if (OpeningChars.Contains(c))
                {
                    chunkStack.Push(c);
                }
                else
                {
                    if (chunkStack.Count == 0)
                    {
                        invalidChar = c;
                        return true;
                    }

                    var currentChunkOpener = chunkStack.Peek();

                    if (c != OpenCloseMap[currentChunkOpener])
                    {
                        invalidChar = c;
                        return true;
                    }

                    chunkStack.Pop();
                }
            }

            invalidChar = '-';
            return false;
        }

        private static long ComputeAutocompleteScore(string l)
        {
            var chunkStack = new Stack<char>();

            foreach (var c in l)
            {
                if (OpeningChars.Contains(c))
                {
                    chunkStack.Push(c);
                }
                else
                {
                    chunkStack.Pop();
                }
            }

            var score = 0L;

            while (chunkStack.Count > 0)
            {
                var topOpener = chunkStack.Pop();

                var nextChar = OpenCloseMap[topOpener];

                score = (score * AutoCompleteFactor) + CharAutoCompleteScoreMap[nextChar];
            }

            return score;
        }
    }
}
