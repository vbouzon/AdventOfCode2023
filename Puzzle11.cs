using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle11
{
    public void Part01()
    {
        Console.WriteLine(Compute(File.ReadAllLines("Puzzle11.txt"), 1) == 9550717);
    }

    public void Part02()
    {
        Console.WriteLine(Compute(File.ReadAllLines("Puzzle11.txt"), 999999) == 648458253817);
    }

    private long Compute(string[] lines, int additionalDistance)
    {
        var emptyRows = Enumerable.Range(0, lines.Length).Where(l => lines[l].All(ch => ch == '.')).ToArray();
        var emptyCols = Enumerable.Range(0, lines[0].Length).Where(c => lines.All(row => row[c] == '.')).ToArray();

        var stars = GetGalaxies(lines).ToArray();

        return GeneratePairs(stars)
            .Select(i => ManhattanDistance(i.Item1, i.Item2, additionalDistance, emptyRows, emptyCols))
            .Sum();
    }

    private long ManhattanDistance(Point point1, Point point2, int expansion, int[] emptyRows, int[] emptyCols)
    {
        return DistanceOnAxis(point1.Y, point2.Y, expansion, emptyRows) +
               DistanceOnAxis(point1.X, point2.X, expansion, emptyCols);
    }

    private static List<(T, T)> GeneratePairs<T>(T[] array)
    {
        return array.SelectMany((x, i) => array.Skip(i + 1).Select(y => (x, y))).ToList();
    }

    private long DistanceOnAxis(long a, long b, int additionWhenEmpty, int[] emptyIndexes)
    {
        return Math.Abs(a - b) + additionWhenEmpty *
            Enumerable.Range((int)Math.Min(a, b), (int)Math.Abs(a - b)).Count(emptyIndexes.Contains);
    }

    private IEnumerable<Point> GetGalaxies(string[] lines)
    {
        return Enumerable.Range(0, lines.Length)
            .SelectMany(y => Enumerable.Range(0, lines[0].Length).Select(x => (x, y)))
            .Where(e => lines[e.y][e.x] == '#')
            .Select(p => new Point(p.x, p.y));
    }
}