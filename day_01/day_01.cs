namespace aoc.day_01
{
    // https://adventofcode.com/2022/day/1
    public class Day_01
    {
        public static void GetResult()
        {
            Console.WriteLine(FindMaxCalories());
            Console.WriteLine(FindMaxNCalories(3));
        }

        private static int FindMaxCalories()
        {
            var cals = GetElfCalories();

            return cals.Max();
        }

        private static int FindMaxNCalories(int n)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n));

            var cals = GetElfCalories();

            cals.Sort();

            var sum = cals.TakeLast(n).Sum();

            return sum;
        }

        private static List<int> GetElfCalories()
        {
            var lines = ReadInputLines();

            var currentElf = 0;
            var elfCalorieCounts = new List<int>();

            elfCalorieCounts.Add(0);

            for (int i = 0; i < lines.Length; i++)
            {
                var lineHasValue = int.TryParse(lines[i], out var calorieCount);

                if (lineHasValue)
                {
                    elfCalorieCounts[currentElf] += calorieCount;
                }
                else
                {
                    elfCalorieCounts.Add(0);
                    currentElf++;
                }
            }

            return elfCalorieCounts;
        } 

        private static string[] ReadInputLines()
        {
            const string fileName = "day_01/day_01_input.txt";

            var lines = File.ReadAllLines(fileName);

            return lines;
        }
    }
}
