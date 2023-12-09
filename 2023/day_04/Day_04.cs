using aoc.common;
using aoc.utils.extensions;

namespace aoc.y2023.day_04
{
    // https://adventofcode.com/2023/day/04
    public class Day_04 : ISolver
    {
        private class Card
        {
            public int Id { get; set; }
            public List<int> WinningNums { get; set; } = new List<int>();
            public List<int> ActualNums { get; set; } = new List<int>();

            public int GetMatchCount()
            {
                return WinningNums.Intersect(ActualNums).Count();
            }

            public int GetScore()
            {
                var count = GetMatchCount();

                return count == 0 ? 0 : (int)Math.Pow(2, count - 1);
            }
        }

        public object Part1(IList<string> lines)
        {
            var tickets = ParseTickets(lines);

            var scores = tickets.Select(t => t.GetScore());

            return scores.Sum();
        }

        public object Part2(IList<string> lines)
        {
            var cards = ParseTickets(lines);

            var cardCount = cards.Count;
            var cardsToProcess = new Queue<Card>(cards);

            while (cardsToProcess.Count > 0)
            {
                var current = cardsToProcess.Dequeue();

                var newCardsCount = current.GetMatchCount();

                if (newCardsCount > 0)
                {
                    cardCount += newCardsCount;

                    var newCards = cards.Skip(current.Id).Take(newCardsCount);

                    foreach (var newCard in newCards)
                    {
                        cardsToProcess.Enqueue(newCard);
                    }
                }
            }

            return cardCount;
        }

        private static List<Card> ParseTickets(IList<string> lines)
        {
            return lines
                .Select(l => l.Split(':').Last())
                .Select(l => l.Split("|"))
                .Select(l => (l[0].ReadAllNumbers<int>().ToList(), l[1].ReadAllNumbers<int>().ToList()))
                .WithIndex()
                .Select(c => new Card { Id = c.Index + 1, WinningNums = c.Item.Item1, ActualNums = c.Item.Item2 })
                .ToList();
        }
    }
}
