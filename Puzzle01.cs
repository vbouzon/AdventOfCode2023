using System.Text.RegularExpressions;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

internal partial class Puzzle01
{
    private static string Numberize(string text)
    {
        string[] units = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
        return units.WithIndex().Aggregate(text, (x, i) => x.Replace(i.Item, i.Item + i.Index + i.Item));
    }

    public void Part01()
    {
        var result =
            File.ReadAllLines("Puzzle01.txt")
                .AsParallel()
                .Select(l => LetterRegex().Replace(l, ""))
                .Select(n => int.Parse(new string(new[] { n.First(), n.Last() })))
                .Sum();

        Console.WriteLine(result == 54916);
    }

    public void Part02()
    {
        var result =
            File.ReadAllLines("Puzzle01.txt")
                .AsParallel()
                .Select(x => Numberize(x.ToLower()))
                .Select(l => LetterRegex().Replace(l, ""))
                .Select(n => int.Parse(new string(new[] { n.First(), n.Last() })))
                .Sum();

        Console.WriteLine(result == 54728);
    }

    [GeneratedRegex("[^0-9.]")]
    private static partial Regex LetterRegex();
}