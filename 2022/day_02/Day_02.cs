using aoc.common;

namespace aoc.y2022.day_02
{
    // https://adventofcode.com/2022/day/2
    public class Day_02
    {
        private enum Rps
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3
        }

        private enum Outcome
        {
            Lose = 0,
            Draw = 3,
            Win = 6
        }

        private class RpsRound
        {
            public Rps Opponent { get; set; }
            public Rps You { get; set; }

            public int GetScore()
            {
                return (int)You + GetRoundScore();
            }

            public int GetRoundScore()
            {
                if (Opponent == You)
                {
                    return (int)Outcome.Draw;
                }

                if (Opponent == Rps.Rock && You == Rps.Paper ||
                    Opponent == Rps.Paper && You == Rps.Scissors ||
                    Opponent == Rps.Scissors && You == Rps.Rock)
                {
                    return (int)Outcome.Win;
                }

                return (int)Outcome.Lose;
            }
        }

        public void Solve()
        {
            Console.WriteLine(GetScore());
            Console.WriteLine(GetScoreV2());
        }

        private static int GetScore()
        {
            return GetRounds().Select(r => r.GetScore()).Sum();
        }

        private static int GetScoreV2()
        {
            return GetRoundsV2().Select(r => r.GetScore()).Sum();
        }

        private static RpsRound GetRound(string roundLine)
        {
            if (roundLine.Length != 3)
            {
                throw new ArgumentException(nameof(roundLine));
            }

            var opponentRps = GetOpponentSelection(roundLine);
            
            var you = roundLine[2];
            Rps youRps;

            switch (you)
            {
                case 'X':
                    youRps = Rps.Rock;
                    break;
                case 'Y':
                    youRps = Rps.Paper;
                    break;
                case 'Z':
                    youRps = Rps.Scissors;
                    break;
                default:
                    throw new Exception($"Unknown you RPS selection: {you}");
            }

            var round = new RpsRound
            {
                Opponent = opponentRps,
                You = youRps
            };

            return round;
        }

        private static RpsRound GetRoundV2(string roundLine)
        {
            if (roundLine.Length != 3)
            {
                throw new ArgumentException(nameof(roundLine));
            }

            var opponentRps = GetOpponentSelection(roundLine);

            var desiredOutcome = roundLine[2];

            Outcome desiredOutcomeEnum;

            switch (desiredOutcome)
            {
                case 'X':
                    desiredOutcomeEnum = Outcome.Lose;
                    break;
                case 'Y':
                    desiredOutcomeEnum = Outcome.Draw;
                    break;
                case 'Z':
                    desiredOutcomeEnum = Outcome.Win;
                    break;
                default:
                    throw new Exception($"Unknown outcome selection: {desiredOutcome}");
            }

            Rps youRps = DetermineWhatToPlay(opponentRps, desiredOutcomeEnum);

            var round = new RpsRound
            {
                Opponent = opponentRps,
                You = youRps
            };

            return round;
            
        }

        private static List<RpsRound> GetRounds()
        {
            var roundLines = GetLines();

            var rounds = roundLines.Select(rl => GetRound(rl)).ToList();

            return rounds;
        }

        private static List<RpsRound> GetRoundsV2()
        {
            var roundLines = GetLines();

            var rounds = roundLines.Select(rl => GetRoundV2(rl)).ToList();

            return rounds;
        }

        private static List<string> GetLines()
        {
            var roundLines = File.ReadAllLines("2022/day_02/input.txt");

            return roundLines.ToList();
        }

        private static Rps GetOpponentSelection(string roundLine)
        {
            var opponent = roundLine[0];

            Rps opponentRps;

            switch (opponent)
            {
                case 'A':
                    opponentRps = Rps.Rock;
                    break;
                case 'B':
                    opponentRps = Rps.Paper;
                    break;
                case 'C':
                    opponentRps = Rps.Scissors;
                    break;
                default:
                    throw new Exception($"Unknown opponent RPS selection: {opponent}");
            }

            return opponentRps;
        }

        private static Rps DetermineWhatToPlay(Rps opponent, Outcome desiredOutcome)
        {
            if (desiredOutcome == Outcome.Draw)
            {
                return opponent;
            }

            if (desiredOutcome == Outcome.Win)
            {
                if (opponent == Rps.Rock)
                {
                    return Rps.Paper;
                }

                if (opponent == Rps.Paper)
                {
                    return Rps.Scissors;
                }

                if (opponent == Rps.Scissors)
                {
                    return Rps.Rock;
                }
            }

            if (desiredOutcome == Outcome.Lose)
            {
                if (opponent == Rps.Rock)
                {
                    return Rps.Scissors;
                }

                if (opponent == Rps.Paper)
                {
                    return Rps.Rock;
                }

                if (opponent == Rps.Scissors)
                {
                    return Rps.Paper;
                }
            }

            throw new Exception($"Unexepcted outcome: {desiredOutcome}");
        }
    }
}
