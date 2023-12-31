using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle10
{
    private static readonly Point[] AllDirections = { Point.ToUp, Point.ToRight, Point.ToDown, Point.ToLeft };

    private static readonly Dictionary<char, Point[]> Movements = new()
    {
        { 'J', new[] { Point.ToUp, Point.ToLeft } },
        { '|', new[] { Point.ToUp, Point.ToDown } },
        { 'L', new[] { Point.ToUp, Point.ToRight } },
        { '7', new[] { Point.ToLeft, Point.ToDown } },
        { 'F', new[] { Point.ToRight, Point.ToDown } },
        { '-', new[] { Point.ToLeft, Point.ToRight } },
        { 'S', new[] { Point.ToUp, Point.ToRight, Point.ToDown, Point.ToLeft } },
        { '.', Array.Empty<Point>() }
    };

    public void Part01()
    {
        var map = GetPoints(File.ReadAllLines("Puzzle10.txt"));
        var steps = ComputeLoopSteps(map);

        Console.WriteLine(steps.Count / 2 == 6828);
    }

    public void Part02()
    {
        var map = GetPoints(File.ReadAllLines("Puzzle10.txt"));
        var steps = ComputeLoopSteps(map);
        Console.WriteLine(map.Keys.Count(position => Inside(position, map, steps)) == 459);
    }

    private static HashSet<Point> ComputeLoopSteps(Dictionary<Point, char> map)
    {
        var steps = new HashSet<Point>();

        var currentPosition = map.Keys.Single(k => map[k] == 'S');
        var movement = AllDirections.First(d => Movements[map[currentPosition + d]].Contains(-d));

        while (true)
        {
            steps.Add(currentPosition);
            currentPosition += movement;
            if (map[currentPosition] == 'S') break;
            movement = Movements[map[currentPosition]].Single(c => c != -movement);
        }

        return steps;
    }

    private static bool Inside(Point position, Dictionary<Point, char> map, HashSet<Point> steps)
    {
        if (steps.Contains(position)) return false;

        var inside = false;
        position += Point.ToRight;
        while (map.ContainsKey(position))
        {
            if (steps.Contains(position) && Movements[map[position]].Contains(Point.ToUp)) inside = !inside;
            position += Point.ToRight;
        }

        return inside;
    }

    private static Dictionary<Point, char> GetPoints(string[] lines)
    {
        return Enumerable.Range(0, lines.Length)
            .SelectMany(y => Enumerable.Range(0, lines[0].Length).Select(x => (x, y)))
            .ToDictionary(p => new Point(p.x, p.y), p => lines[p.y][p.x]);
    }
}