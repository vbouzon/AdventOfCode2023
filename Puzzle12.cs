using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

internal class Puzzle12
{
    private static IEnumerable<(string, int[])> GetLinesWithGroups()
    {
        var lines = File.ReadAllLines("Puzzle12.txt");
        return lines
            .Select(l => l.Split(" "))
            .Select(l => (l[0], l[1].Split(',').Select(int.Parse).ToArray()));
    }

    public void Part01()
    {
        var cache = new Dictionary<string, long>();
        var result = GetLinesWithGroups().Sum(l => Compute(l.Item1, l.Item2, cache));

        Console.WriteLine(result == 7025);
    }

    public void Part02()
    {
        var cache = new Dictionary<string, long>();
        var result = GetLinesWithGroups().Sum(l =>
            Compute(new string(Clone(l.Item1.ToArray(), new[] { '?' }, 5)),
                Clone(l.Item2.ToArray(), Array.Empty<int>(), 5), cache
            ));

        Console.WriteLine(result == 11461095383315);
    }

    private T[] Clone<T>(T[] original, T[] separators, int count)
    {
        return Enumerable.Range(0, count - 1)
            .Select(_ => original)
            .Aggregate(original, (arg1, arg2) => arg1.Concat(separators).Concat(arg2).ToArray());
    }

    private long Compute(string line, int[] groups, Dictionary<string, long> cache)
    {
        var keyCache = line + ";" + string.Join(",", groups.Select(n => n.ToString()));
        if (cache.TryGetValue(keyCache, out var cachedResult)) return cachedResult;

        var result = line.FirstOrDefault() switch
        {
            '#' => Process(line, groups, cache),
            '.' => Compute(line[1..], groups, cache),
            '?' => Compute("." + line[1..], groups, cache) + Compute("#" + line[1..], groups, cache),
            _ => groups.Length == 0 ? 1 : 0
        };

        cache[keyCache] = result;
        return result;
    }

    private long Process(string line, int[] groups, Dictionary<string, long> cache)
    {
        return (groups.Length != 0, line) switch
        {
            (false, _) => 0,
            var (_, l) when l.TakeWhile(s => s is '?' or '#').Count() < groups[0] => 0,
            var (_, l) when l.Length > groups[0] && l[groups[0]] == '#' => 0,
            var (_, l) when l.Length == groups[0] => Compute("", groups.Skip(1).ToArray(), cache),
            var (_, l) => Compute(l[(groups[0] + 1)..], groups.Skip(1).ToArray(), cache)
        };
    }
}