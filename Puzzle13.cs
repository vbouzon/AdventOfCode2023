namespace AdventOfCode2023;

internal class Puzzle13
{
    private static string[] Rotate(string[] lines)
    {
        return Enumerable.Range(0, lines[0].Length)
            .Select(i => new string(Enumerable.Range(0, lines.Length).Select(j => lines[j][i]).ToArray())).ToArray();
    }

    public void Part02()
    {
        var patterns = File.ReadAllText("Puzzle13.txt").Split("\r\n\r\n").Select(l => l.Split("\r\n")).ToArray();
        var resultSum = patterns
            .Select(pattern =>
            {
                var result = CalculateSymmetry(pattern, CountDifferences, 1);
                return result > 0 ? result * 100 : CalculateSymmetry(Rotate(pattern), CountDifferences, 1);
            })
            .Sum();

        Console.WriteLine(resultSum == 22906);
    }

    public void Part01()
    {
        var patterns = File.ReadAllText("Puzzle13.txt").Split("\r\n\r\n").Select(l => l.Split("\r\n")).ToArray();
        var resultSum = patterns
            .Select(pattern =>
            {
                var result = CalculateSymmetry(pattern, CountDifferences, 0);
                return result > 0 ? result * 100 : CalculateSymmetry(Rotate(pattern), CountDifferences, 0);
            })
            .Sum();

        Console.WriteLine(resultSum == 27505);
    }

    private static int CountDifferences(string str1, string str2)
    {
        if (str1.Length != str2.Length) throw new ArgumentException("Strings must be of the same length");

        return str1.Zip(str2, (c1, c2) => c1 != c2 ? 1 : 0).Sum();
    }

    private static int CalculateSymmetry(string[] verticals, Func<string, string, int> comparison, int maxDifference,
        int start = 1)
    {
        if (start >= verticals.Length)
            return 0;
        var totalDifference = 0;
        for (var i = start; i < verticals.Length; i++)
        {
            var comp = comparison(verticals[i], verticals[i - 1]);
            if (comp > maxDifference) continue;
            totalDifference += comp;

            var size = 1;
            for (;;)
            {
                var top = i - size - 1;
                var bottom = i + size;

                var end = top < 0 || bottom >= verticals.Length;
                if (end)
                {
                    if (totalDifference == maxDifference)
                        return i;

                    break;
                }

                totalDifference += comparison(verticals[bottom], verticals[top]);
                if (totalDifference > maxDifference)
                    break;

                size++;
            }

            return CalculateSymmetry(verticals, comparison, maxDifference, i + 1);
        }

        return 0;
    }
}