using Aoc.Common;
using Aoc.Utils;
using Aoc.Utils.Extensions;

namespace Aoc.y2023.day_01
{
    // https://adventofcode.com/2023/day/01
    public class Day_01 : ISolver
    {
        public object Part1(IList<string> lines)
        {
            var numLines = lines
                .Select(l => l.Where(c => c.IsNumber()).ToArray());

            var sumTotal = numLines
                .Select(nl => int.Parse($"{nl[0]}{nl[^1]}"))
                .Sum();

            return sumTotal;
        }

        public object Part2(IList<string> lines)
        {
            var numWords = new Dictionary<string, int>()
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
                { "four", 4 },
                { "five", 5 },
                { "six", 6 },
                { "seven", 7 },
                { "eight", 8 },
                { "nine", 9 }
            };

            var nums = new List<int>();

            foreach (var l in lines)
            {
                var firstNum = "";
                var secondNum = "";

                for (int i = 0; i < l.Length; i++)
                {
                    if (l[i].IsNumber())
                    {
                        firstNum = l[i].ToString();
                        break;
                    }

                    var substring = l.Substring(i);

                    bool foundWord = false;
                    foreach (var w in numWords)
                    {
                        if (substring.StartsWith(w.Key))
                        {
                            firstNum = w.Value.ToString();
                            foundWord = true;
                            break;
                        }
                    }

                    if (foundWord)
                    {
                        break;
                    }
                }

                for (int i = l.Length - 1; i >= 0; i--)
                {
                    if (l[i].IsNumber())
                    {
                        secondNum = l[i].ToString();
                        break;
                    }

                    var substring = l.Substring(i);

                    bool foundWord = false;
                    foreach (var w in numWords)
                    {
                        if (substring.StartsWith(w.Key))
                        {
                            secondNum = w.Value.ToString();
                            foundWord = true;
                            break;
                        }
                    }

                    if (foundWord)
                    {
                        break;
                    }
                }

                nums.Add(int.Parse(firstNum + secondNum));

            }

            return nums.Sum();
        }
    }
}
