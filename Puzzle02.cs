using System.Text.RegularExpressions;

namespace AdventOfCode2023;

internal partial class Puzzle02
{
    private static readonly string[] Separator = { "; " };

    public void Part01()
    {
        var games = ParseText(File.ReadAllLines("Puzzle02.txt"));

        Set bag = new()
        {
            NumberAndColors =
            [
                new NumberAndColor { Number = 12, Color = Color.Red },
                new NumberAndColor { Number = 13, Color = Color.Green },
                new NumberAndColor { Number = 14, Color = Color.Blue }
            ]
        };

        var sum = games.Where(g => g.CanPlayWithBag(bag)).Aggregate(0, (a, b) => a + b.Id);
        Console.WriteLine(sum == 2416);
    }

    public void Part02()
    {
        var games = ParseText(File.ReadAllLines("Puzzle02.txt"));
        Console.WriteLine(games.Sum(g => g.Power()) == 63307);
    }

    private List<Game> ParseText(string[] lines)
    {
        return (from line in lines
            select GameRegex().Match(line)
            into match
            let gameId = int.Parse(match.Groups[1].Value)
            let setInfo = match.Groups[2].Value
            let sets = setInfo.Split(Separator, StringSplitOptions.None)
                .Select(token => new Set
                {
                    NumberAndColors = NumberAndColorsRegex().Matches(token)
                        .Select(setMatch => new NumberAndColor
                        {
                            Number = int.Parse(setMatch.Groups[1].Value),
                            Color = Enum.Parse<Color>(setMatch.Groups[2].Value, true)
                        })
                        .ToList()
                })
                .ToList()
            select new Game { Id = gameId, Sets = sets }).ToList();
    }

    [GeneratedRegex(@"Game (\d+): (.+)")]
    private static partial Regex GameRegex();

    [GeneratedRegex(@"(\d+) (\w+)")]
    private static partial Regex NumberAndColorsRegex();

    private enum Color
    {
        Red,
        Green,
        Blue
    }

    private record Set
    {
        public List<NumberAndColor> NumberAndColors { get; init; } = [];

        public override string ToString()
        {
            return string.Join(",", NumberAndColors);
        }
    }

    private record NumberAndColor
    {
        public int Number { get; init; }
        public Color Color { get; init; }

        public override string ToString()
        {
            return Number + " " + Color;
        }
    }

    private class Game
    {
        public int Id { get; init; }
        public required List<Set> Sets { get; init; }

        public bool CanPlayWithBag(Set bag)
        {
            return !(from setItem in Sets
                from item in setItem.NumberAndColors
                let bagSet = bag.NumberAndColors.FirstOrDefault(b => b.Color == item.Color)
                where bagSet == null || bagSet.Number < item.Number
                select item).Any();
        }

        private IEnumerable<NumberAndColor> MinimumNecessarySets()
        {
            return Sets.SelectMany(s => s.NumberAndColors)
                .GroupBy(s => s.Color)
                .Select(g => g.OrderByDescending(s => s.Number).First());
        }

        public int Power()
        {
            return MinimumNecessarySets()
                .Select(s => s.Number)
                .Aggregate(1, (a, b) => a * b);
        }
    }
}