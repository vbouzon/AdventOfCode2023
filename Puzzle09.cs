using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public partial class Puzzle09
{
    public void Part01()
    {
        var suites = GetSuites();
        Console.WriteLine(suites.Aggregate(new BigInteger(0), (i, suite) => i + suite.GetNext()) == 1702218515);
    }

    public void Part02()
    {
        var suites = GetSuites();
        Console.WriteLine(suites.Aggregate(new BigInteger(0), (i, suite) => i + suite.GetPrevious()) == 925);
    }

    private static IEnumerable<Suite> GetSuites()
    {
        var lines = File.ReadAllLines("Puzzle09.txt");

        var suites = lines
            .Select(l => new
            {
                Line = l,
                RegexResult = LineRegex().Matches(l)
            }).Select(line => new Suite(line.RegexResult.Select(t => BigInteger.Parse(t.Value)).ToArray()));

        return suites;
    }

    private class Suite(BigInteger[] numbers)
    {
        private Suite GetBottom()
        {
            return new Suite(numbers.Zip(numbers.Skip(1)).Select(p => p.Second - p.First).ToArray());
        }

        public BigInteger GetNext()
        {
            if (numbers.All(i => i == 0))
                return 0;

            return GetBottom().GetNext() + numbers[^1];
        }

        public BigInteger GetPrevious()
        {
            if (numbers.All(i => i == 0))
                return 0;

            return numbers[0] - GetBottom().GetPrevious();
        }

        public override string ToString()
        {
            return $"{string.Join(" ", numbers)} {GetNext()}";
        }
    }

    [GeneratedRegex(@"([-\d]+)")]
    private static partial Regex LineRegex();
}