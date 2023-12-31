using System.Text.RegularExpressions;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public partial class Puzzle05
{
    private static TResult Browse<TResult>(TResult seed, Dictionary<string, Mapper> mappers,
        Func<TResult, Mapper, TResult> map)
    {
        var i = seed;
        var curMap = mappers["seed"];
        do
        {
            i = map(i, curMap);
        } while ((curMap = mappers.GetValueOrDefault(curMap.To)) != null);

        return i;
    }

    public void Part01()
    {
        var lines = File.ReadAllText("Puzzle05.txt");
        var seedHeader = ParseLines(lines, out var mappers);
        var seed = NumberRegex().Matches(seedHeader);

        Console.WriteLine(seed.Select(s => decimal.Parse(s.Value))
            .Select(s => Browse(s, mappers, (i, c) => c.MapX(i))).Min() == 265018614);
    }

    private static string ParseLines(string lines, out Dictionary<string, Mapper> mappers)
    {
        var parts = lines.Split("\r\n\r\n");
        var seedHeader = parts[0];

        mappers = parts
            .Skip(1)
            .Select(part =>
            {
                var partParts = part.Split("\r\n");
                var header = MyRegex().Match(partParts[0]).Groups;
                var from = header[1].Value;
                var to = header[2].Value;

                return new Mapper(from, to, partParts.Skip(1).ToArray());
            })
            .ToDictionary(mapper => mapper.From);
        return seedHeader;
    }

    public void Part02()
    {
        var lines = File.ReadAllText("Puzzle05.txt");
        var seedHeader = ParseLines(lines, out var mappers);
        var seed = SeedRegex().Matches(seedHeader);

        var seeds = seed.Select(s =>
            AoCRange.FromDistance(decimal.Parse(s.Groups[1].Value), decimal.Parse(s.Groups[2].Value))).ToArray();

        Console.WriteLine(seeds.SelectMany(s => Browse(new[] { s }, mappers, (i, c) => MapRanges(i, [.. c.Map])))
            .Min(s => s.Start) == 63179500);
    }

    private static AoCRange[] MapRanges(AoCRange[] ranges, MapFunction[] mapFunctions)
    {
        List<AoCRange> result = [];

        var tmp = SplitRanges(ranges, mapFunctions);

        foreach (var range in tmp)
        {
            var mapper = mapFunctions.SingleOrDefault(m => m.SourceAocRange.IsSubset(range));

            result.Add(mapper != null ? new AoCRange(mapper.Map(range.Start), mapper.Map(range.End)) : range);
        }

        return [.. result];
    }

    private static List<AoCRange> SplitRanges(AoCRange[] ranges, MapFunction[] mapFunctions)
    {
        List<AoCRange> tmp = [];
        var splitter = mapFunctions.OrderBy(m => m.SourceAocRange.Start).Select(s => s.SourceAocRange).ToArray();

        foreach (var range in ranges.OrderBy(r => r.Start))
        {
            var found = false;
            foreach (var split in splitter)
            {
                var splitResult = range.Split(split);
                if (splitResult.Length >= 2)
                    found = true;

                if (splitResult.Length >= 2) tmp.AddRange(splitResult.Where(s => tmp.Any(x => x.IsSubset(s)) == false));
            }

            if (found == false)
                tmp.Add(range);
        }

        return tmp.Distinct().ToList();
    }

    [GeneratedRegex(@"(\w+)-to-(\w+)")]
    private static partial Regex MyRegex();

    [GeneratedRegex(@"(\d+)")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"(\d+) (\d+)")]
    private static partial Regex SeedRegex();


    public class MapFunction(decimal source, decimal destination, decimal distance)
    {
        public AoCRange SourceAocRange { get; } = AoCRange.FromDistance(source, distance);
        private AoCRange DestinationAocRange { get; } = AoCRange.FromDistance(destination, distance);

        public bool Is(decimal x)
        {
            return SourceAocRange.Contains(x);
        }

        public decimal Map(decimal x)
        {
            return DestinationAocRange.Start + (x - SourceAocRange.Start);
        }

        public override string ToString()
        {
            return $"{SourceAocRange}->{DestinationAocRange}";
        }
    }

    public partial record Mapper
    {
        public Mapper(string from, string to, string[] lines)
        {
            From = from;
            To = to;
            ParseLines(lines);
        }

        public List<MapFunction> Map { get; } = [];
        public string From { get; }
        public string To { get; }

        public decimal MapX(decimal x)
        {
            foreach (var mapFunction in Map.Where(mapFunction => mapFunction.Is(x)))
                return mapFunction.Map(x);

            return x;
        }

        private void ParseLines(string[] lines)
        {
            foreach (var line in lines)
            {
                var match = LineRegex().Match(line);
                var to = decimal.Parse(match.Groups[1].Value);
                var from = decimal.Parse(match.Groups[2].Value);
                var distance = decimal.Parse(match.Groups[3].Value);

                Map.Add(new MapFunction(from, to, distance));
            }
        }

        [GeneratedRegex(@"(\d+) (\d+) (\d+)")]
        private static partial Regex LineRegex();
    }
}