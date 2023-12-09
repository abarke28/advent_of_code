using aoc.common;
using aoc.utils.extensions;

namespace aoc.y2023.day_07
{
    // https://adventofcode.com/2023/day/07
    public class Day_07 : ISolver
    {
        private class Hand : IComparable<Hand>
        {
            public string Cards { get; set; } = string.Empty;
            public long Bid { get; set; }

            public int CompareTo(Hand? other)
            {
                var i = 0;

                while (true)
                {
                    if (CardRankings[Cards[i]] == CardRankings[other!.Cards[i]])
                    {
                        i++;
                        continue;
                    }

                    return CardRankings[Cards[i]] < CardRankings[other!.Cards[i]] ? -1 : 1;
                }
            }
        }

        private class SpecialHandComparer : IComparer<Hand>
        {
            public int Compare(Hand? x, Hand? y)
            {
                if (x!.Cards.Equals(y!.Cards)) return 0;

                var i = 0;

                while (true)
                {
                    if (SpecialCardRankings[x!.Cards[i]] == SpecialCardRankings[y!.Cards[i]])
                    {
                        i++;
                        continue;
                    }

                    return SpecialCardRankings[x!.Cards[i]] < SpecialCardRankings[y!.Cards[i]] ? -1 : 1;
                }
            }
        }

        private const char Joker = 'J';

        private static Func<string, bool> IsFiveOfAKind = (s) =>
        {
            return s.ToHashSet().Count() == 1;
        };

        private static Func<string, bool> IsFourOfAKind = (s) =>
        {
            return s.GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count())
                    .Any(kvp => kvp.Value == 4);
        };
        
        private static Func<string, bool> IsFullHouse = (s) =>
        {
            var charCounts = s.GroupBy(c => c)
                              .ToDictionary(g => g.Key, g => g.Count());

            return charCounts.Any(kvp => kvp.Value == 2) && charCounts.Any(kvp => kvp.Value == 3);
        };
        
        private static Func<string, bool> IsThreeOfAKind = (s) =>
        {
            return s.GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count())
                    .Any(kvp => kvp.Value == 3);
        };
        
        private static Func<string, bool> IsTwoPair = (s) =>
        {
            return s.GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count())
                    .Where(kvp => kvp.Value == 2)
                    .Count() == 2;
        };
        
        private static Func<string, bool> IsOnePair = (s) =>
        {
            return s.GroupBy(c => c)
                    .ToDictionary(g => g.Key, g => g.Count())
                    .Where(kvp => kvp.Value == 2)
                    .Count() == 1;
        };
        
        private static Func<string, bool> IsSpecialFiveOfAKind = (s) =>
        {
            var cardCounts = s.GroupBy(c => c)
                              .ToDictionary(g => g.Key, g => g.Count());

            var hasJoker = s.Contains(Joker);
            var jokerCount = hasJoker ? cardCounts[Joker] : 0;

            return 
                cardCounts.Any(cc => cc.Value == 5) ||
                cardCounts.Any(cc => cc.Key != Joker && cc.Value + jokerCount == 5);

        };
        
        private static Func<string, bool> IsSpecialFourOfAKind = (s) =>
        {
            var cardCounts = s.GroupBy(c => c)
                              .ToDictionary(g => g.Key, g => g.Count());

            var hasJoker = s.Contains(Joker);
            var jokerCount = hasJoker ? cardCounts[Joker] : 0;

            return
                cardCounts.Any(cc => cc.Value == 4) ||
                cardCounts.Any(cc => cc.Key != Joker && cc.Value + jokerCount == 4);
        };
        
        private static Func<string, bool> IsSpecialFullHouse = (s) =>
        {
            var hasJoker = s.Contains(Joker);
            var cardCounts = s.GroupBy(c => c)
                              .ToDictionary(g => g.Key, g => g.Count());

            return
                IsFullHouse(s) ||
                (cardCounts.Where(cc => cc.Key != Joker && cc.Value == 2).Count() == 2 && hasJoker);
        };
        
        private static Func<string, bool> IsSpecialThreeOfAKind = (s) =>
        {
            var cardCounts = s.GroupBy(c => c)
                              .ToDictionary(g => g.Key, g => g.Count());

            var hasJoker = s.Contains(Joker);
            var jokerCount = hasJoker ? cardCounts[Joker] : 0;

            return
                cardCounts.Any(cc => cc.Value == 3) ||
                cardCounts.Any(cc => cc.Key != Joker && cc.Value + jokerCount == 3);
        };
        
        private static Func<string, bool> IsSpecialTwoPair = (s) =>
        {
            var hasJoker = s.Contains(Joker);
            var cardCounts = s.GroupBy(c => c)
                              .ToDictionary(g => g.Key, g => g.Count());

            return
                IsTwoPair(s) ||
                (cardCounts.Where(cc => cc.Key != Joker && cc.Value == 2).Count() == 1 && hasJoker);
        };
        
        private static Func<string, bool> IsSpecialOnePair = (s) =>
        {
            return IsOnePair(s) || s.Contains(Joker);
        };
        
        private static Dictionary<char, int> CardRankings = new Dictionary<char, int>
        {
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'T', 10 },
            { 'J', 11 },
            { 'Q', 12 },
            { 'K', 13 },
            { 'A', 14 }
        };
        
        private static Dictionary<char, int> SpecialCardRankings = new Dictionary<char, int>
        {
            { 'J', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'T', 10 },
            { 'Q', 12 },
            { 'K', 13 },
            { 'A', 14 }
        };

        public object Part1(IList<string> lines)
        {
            var hands = ParseHands(lines);

            var fiveOfAKinds = new List<Hand>();
            var fourOfAKinds = new List<Hand>();
            var fullHouses = new List<Hand>();
            var threeOfAKinds = new List<Hand>();
            var twoPairs = new List<Hand>();
            var onePairs = new List<Hand>();
            var highCards = new List<Hand>();

            foreach (var hand in hands)
            {
                if (IsFiveOfAKind(hand.Cards)) fiveOfAKinds.Add(hand);
                else if (IsFourOfAKind(hand.Cards)) fourOfAKinds.Add(hand);
                else if (IsFullHouse(hand.Cards)) fullHouses.Add(hand);
                else if (IsThreeOfAKind(hand.Cards)) threeOfAKinds.Add(hand);
                else if (IsTwoPair(hand.Cards)) twoPairs.Add(hand);
                else if (IsOnePair(hand.Cards)) onePairs.Add(hand);
                else highCards.Add(hand);
            }

            highCards.Sort();
            onePairs.Sort();
            twoPairs.Sort();
            threeOfAKinds.Sort();
            fullHouses.Sort();
            fourOfAKinds.Sort();
            fiveOfAKinds.Sort();

            var sortedCards = new List<Hand>(hands.Count);

            sortedCards.AddRange(highCards);
            sortedCards.AddRange(onePairs);
            sortedCards.AddRange(twoPairs);
            sortedCards.AddRange(threeOfAKinds);
            sortedCards.AddRange(fullHouses);
            sortedCards.AddRange(fourOfAKinds);
            sortedCards.AddRange(fiveOfAKinds);

            var rankedCards = sortedCards.WithIndex(indexOffset: 1);
            var winnings = rankedCards.Select(c => c.Index * c.Item.Bid);

            return winnings.Sum();
        }

        public object Part2(IList<string> lines)
        {
            var hands = ParseHands(lines);

            var fiveOfAKinds = new List<Hand>();
            var fourOfAKinds = new List<Hand>();
            var fullHouses = new List<Hand>();
            var threeOfAKinds = new List<Hand>();
            var twoPairs = new List<Hand>();
            var onePairs = new List<Hand>();
            var highCards = new List<Hand>();

            foreach (var hand in hands)
            {
                if (IsSpecialFiveOfAKind(hand.Cards)) fiveOfAKinds.Add(hand);
                else if (IsSpecialFourOfAKind(hand.Cards)) fourOfAKinds.Add(hand);
                else if (IsSpecialFullHouse(hand.Cards)) fullHouses.Add(hand);
                else if (IsSpecialThreeOfAKind(hand.Cards)) threeOfAKinds.Add(hand);
                else if (IsSpecialTwoPair(hand.Cards)) twoPairs.Add(hand);
                else if (IsSpecialOnePair(hand.Cards)) onePairs.Add(hand);
                else highCards.Add(hand);
            }

            var specialComparer = new SpecialHandComparer();

            highCards.Sort(specialComparer);
            onePairs.Sort(specialComparer);
            twoPairs.Sort(specialComparer);
            threeOfAKinds.Sort(specialComparer);
            fullHouses.Sort(specialComparer);
            fourOfAKinds.Sort(specialComparer);
            fiveOfAKinds.Sort(specialComparer);

            var sortedCards = new List<Hand>(hands.Count);

            sortedCards.AddRange(highCards);
            sortedCards.AddRange(onePairs);
            sortedCards.AddRange(twoPairs);
            sortedCards.AddRange(threeOfAKinds);
            sortedCards.AddRange(fullHouses);
            sortedCards.AddRange(fourOfAKinds);
            sortedCards.AddRange(fiveOfAKinds);

            var rankedCards = sortedCards.WithIndex(indexOffset: 1);
            var winnings = rankedCards.Select(c => c.Index * c.Item.Bid);

            return winnings.Sum();
        }

        private static List<Hand> ParseHands(IList<string> lines)
        {
            return lines
                .Select(l => l.Split(' '))
                .Select(l => new Hand { Cards = l[0], Bid = long.Parse(l[1]) })
                .ToList();
        }
    }
}
