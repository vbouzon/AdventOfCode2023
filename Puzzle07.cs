using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public partial class Puzzle07
{
    public void Part01()
    {
        var orderedValues = "AKQJT98765432".Reverse().Aggregate("", (a, b) => a + b);
        var lines = File.ReadAllLines("Puzzle07.txt");

        var hands = lines
            .Select(line => new
            {
                Line = line,
                Regex = LineRegex().Match(line)
            }).Select(x => new CamelHand(x.Regex.Groups[1].Value.ToString(),
                int.Parse(x.Regex.Groups[2].Value)));

        Console.WriteLine(hands.OrderBy(x => x.GlobalPower(false)).ThenBy(x => x.LocalPower(orderedValues))
            .Select((hand, rank) => hand.Bid * (rank + 1)).Sum() == 248396258);
    }

    public void Part02()
    {
        var orderedValues = "AKQT98765432J".Reverse().Aggregate("", (a, b) => a + b);
        var lines = File.ReadAllLines("Puzzle07.txt");

        var hands = lines
            .Select(l => new
            {
                Line = l,
                Regex = Regex.Match(l, @"(\w+) (\d+)")
            }).Select(x => new CamelHand(x.Regex.Groups[1].Value.ToString(),
                int.Parse(x.Regex.Groups[2].Value)));

        Console.WriteLine(hands.OrderBy(x => x.GlobalPower(true)).ThenBy(x => x.LocalPower(orderedValues))
            .Select((hand, rank) => hand.Bid * (rank + 1)).Sum() == 246436046);
    }

    [GeneratedRegex(@"(\w+) (\d+)")]
    private static partial Regex LineRegex();

    private enum HandPattern
    {
        // ReSharper disable UnusedMember.Local
        FiveOfKind = 50000,
        FourOfKind = 41000,
        FullHouse = 32000,
        ThreeOfKind = 31100,
        TwoPair = 22100,
        OnePair = 21110,

        HighCard = 11111
        // ReSharper restore UnusedMember.Local
    }

    private record CamelCard(char Value)
    {
        public int Power(string orderedValues)
        {
            return orderedValues.IndexOf(Value);
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }

    private record CamelHand
    {
        public CamelHand(string cardString, int bid)
        {
            Cards = cardString.Select(c => new CamelCard(c)).ToArray();
            Bid = bid;
        }

        private CamelCard[] Cards { get; }
        private string CardString => Cards.Select(c => c.ToString()).Aggregate((a, b) => a + b);
        public int Bid { get; }

        public int GlobalPower(bool includeJoker)
        {
            return (int)BestPattern(includeJoker).Pattern;
        }

        private (HandPattern Pattern, char Value) BestPattern(bool jokerRule)
        {
            var t = jokerRule ? SimplifyHand().GetMasked() : GetMasked();
            return ((HandPattern)t.Item2, t.Item1[0]);
        }

        private (string, int) GetMasked()
        {
            var t = CardString.GroupBy(c => c)
                .OrderByDescending(g => g.Count())
                .Aggregate(("", ""), (a, b) => (a.Item1 + b.Key, a.Item2 + b.Count()));

            return (t.Item1, int.Parse(t.Item2.PadRight(5, '0')));
        }

        private CamelHand SimplifyHand()
        {
            if (CardString.Contains("J") == false)
                return this;

            if (CardString == "JJJJJ")
                return new CamelHand("AAAAA", Bid);

            var bestPatternWithoutJokers = GetMasked().Item1.First(c => c != 'J');
            return new CamelHand(CardString.Replace('J', bestPatternWithoutJokers), Bid);
        }

        public int LocalPower(string orderedValues)
        {
            return Cards.Reverse().WithIndex().Aggregate(0,
                (i, card) =>
                    i + card.Item1.Power(orderedValues) * (int)BigInteger.Pow(orderedValues.Length + 1, card.Item2));
        }

        public override string ToString()
        {
            return $"{Cards.Select(c => c.ToString()).Aggregate((a, b) => a + b)} {Bid}";
        }
    }
}