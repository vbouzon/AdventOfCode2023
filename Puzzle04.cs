using System.Text.RegularExpressions;

namespace AdventOfCode2023;

internal partial class Puzzle04
{
    private static readonly string[] Separator = { "|" };

    public void Part01()
    {
        var games = ParseText(File.ReadAllLines("Puzzle04.txt"));
        var result = games.Sum(c => c.GetPoint());

        Console.WriteLine(result == 32001);
    }

    private List<Card> ParseText(string[] lines)
    {
        return (from line in lines
            select CardRegex().Match(line)
            into match
            let gameId = int.Parse(match.Groups[1].Value)
            let setInfo = match.Groups[2].Value
            let sets = setInfo.Split(Separator, StringSplitOptions.None)
                .Select(token => new Side
                {
                    Numbers = NumbersRegex().Matches(token)
                        .Select(setMatch => int.Parse(setMatch.Groups[1].Value))
                        .ToList()
                })
                .ToList()
            select new Card { Id = gameId, Sides = sets }).ToList();
    }

    public void Part02()
    {
        var games = ParseText(File.ReadAllLines("Puzzle04.txt"));
        var bag = games.GroupBy(c => c.Id).ToDictionary(k => k.Key, _ => 1);

        var i = 1;

        var maxId = games.Max(g => g.Id);
        while (i < maxId)
        {
            bag = games.Single(c => c.Id == i).Win(games, bag);
            i++;
        }

        Console.WriteLine(bag.Values.Sum() == 5037841);
    }

    [GeneratedRegex(@"Card[ ]*(\d+): (.+)")]
    private static partial Regex CardRegex();

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex NumbersRegex();

    private class Card
    {
        public required List<Side> Sides { get; internal init; }
        public required int Id { get; internal init; }

        private IEnumerable<int> ReturnCommonNumber()
        {
            return Sides[0].Numbers.Intersect(Sides[1].Numbers);
        }

        public int GetPoint()
        {
            return (int)Math.Pow(2, ReturnCommonNumber().Count() - 1);
        }

        public Dictionary<int, int> Win(List<Card> cards, Dictionary<int, int> bag)
        {
            var sameCard = bag[Id];
            var commonNumber = ReturnCommonNumber().Count();
            var toCopy = cards.Where(i => i.Id > Id).Take(commonNumber).ToArray();
            foreach (var card in toCopy) bag[card.Id] += sameCard;

            return bag;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    private class Side
    {
        public required List<int> Numbers { get; init; }
    }
}