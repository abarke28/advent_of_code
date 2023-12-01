using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2023.day_01
{
    // https://adventofcode.com/2023/day/01
    public class Day_01 : ISolver
    {
        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2023/day_01/input2.txt");

            // Part 1
            var numLines = lines
                .Select(l => l.Where(c => c.IsNumber()).ToArray());

            var sumTotal = numLines
                .Select(nl => int.Parse($"{nl[0]}{nl[^1]}"))
                .Sum();

            Console.WriteLine(sumTotal);
            
            // Part 2
            var chars = new Dictionary<char, int>()
            {
                { '1', 1 },
                { '2', 2 },
                { '3', 3 },
                { '4', 4 },
                { '5', 5 },
                { '6', 6 },
                { '7', 7 },
                { '8', 8 },
                { '9', 9 },
                { '0', 0 },
            };

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
                    if (chars.ContainsKey(l[i]))
                    {
                        firstNum = chars[l[i]].ToString();
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
                    if (chars.ContainsKey(l[i]))
                    {
                        secondNum = chars[l[i]].ToString();
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

            Console.WriteLine(nums.Sum());
        }
    }
}
