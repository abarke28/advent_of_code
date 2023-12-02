using aoc.common;
using aoc.utils;
using aoc.utils.extensions;

namespace aoc.y2023.day_02
{
    // https://adventofcode.com/2023/day/02
    public class Day_02 : ISolver
    {
        public class Hand
        {
            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }

            public bool IsPossible(int redCount, int greenCount, int blueCount)
            {
                return redCount >= Red && greenCount>= Green && blueCount >= Blue;
            }
        }

        public class Game
        {
            public int Id { get; set; }
            public List<Hand> Hands { get; set; } = new List<Hand>();

            public bool IsPossible(int redCount, int greenCount,int blueCount)
            {
                return Hands.All(h => h.IsPossible(redCount, greenCount, blueCount));
            }

            public int GetPower()
            {
                var minRed = Hands.Max(h => h.Red);
                var minGreen = Hands.Max(h => h.Green);
                var minBlue = Hands.Max(h => h.Blue);

                return minRed * minGreen * minBlue;
            }
        }

        public void Solve()
        {
            var lines = FileUtils.ReadAllLines("2023/day_02/input.txt");

            Part1(lines);
            Part2(lines);
        }

        private void Part1(List<string> lines)
        {
            const int RedCount = 12;
            const int GreenCount = 13;
            const int BlueCount = 14;

            var games = GetGames(lines);

            var possibleGames = games.Where(g => g.IsPossible(RedCount, GreenCount, BlueCount));

            Console.WriteLine(possibleGames.Sum(g => g.Id));
        }

        private void Part2(List<string> lines)
        {
            var games = GetGames(lines);

            var powers = games.Select(g => g.GetPower());

            Console.WriteLine(powers.Sum());
        }

        private List<Game> GetGames(IList<string> lines)
        {
            var games = new List<Game>();
            
            foreach (var l in lines)
            {
                var game = new Game();

                game.Id = l.Split(':')[0].ReadAllNumbers().Single();

                var handStrings = l.Split(':')[1].Split(';');

                foreach (var handString in handStrings)
                {
                    var hand = new Hand();
                    var ballCounts = handString.Split(',');

                    foreach (var ballCount in ballCounts)
                    {
                        var count = ballCount.ReadAllNumbers().Single();
                        
                        if (ballCount.Contains("red"))
                        {
                            hand.Red += count;
                        }
                        else if (ballCount.Contains("blue"))
                        {
                            hand.Blue += count;
                        }
                        else if (ballCount.Contains("green"))
                        {
                            hand.Green += count;
                        }
                    }

                    game.Hands.Add(hand);
                }

                games.Add(game);
            }

            return games;
        }
    }
}
