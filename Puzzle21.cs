using System.Diagnostics;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle21
{
    public void Part01()
    {
        var map = new Map20(File.ReadAllLines("Puzzle21.txt"));


        var points = new List<Point>();
        var startingPoint = map.GetPoints('S').Single();
        points.Add(startingPoint);
        for (var i = 0; i < 64; i++) points = points.SelectMany(p => map.GetAllPossibleMoves(p)).Distinct().ToList();

        Console.WriteLine(points.Distinct().Count() == 3658);
    }

    public void Part02()
    {
        var map = new Map20(File.ReadAllLines("Puzzle21.txt"));

        var gridSize = map.Width;
        HashSet<Point> points = [];
        var start = map.GetPoints('S').Single();
        points.Add(new Point(start.X, start.Y));

        var pointsAfterIterations = new List<long>();

        var firstIteration = gridSize / 2;
        var secondIteration = firstIteration + gridSize;
        var thirdIteration = secondIteration + gridSize;

        var steps = 0;
        while (steps <= thirdIteration)
        {
            points = points.SelectMany(p => map.GetAllPossibleMoves(p, true)).ToHashSet();
            steps++;

            if (steps == firstIteration || steps == secondIteration || steps == thirdIteration)
                pointsAfterIterations.Add(points.Count);
        }

        var fX = Helpers.FindQuadratic(pointsAfterIterations);

        for (var i = 0; i < 3; i++) Debug.Assert(fX(i) == pointsAfterIterations[i]);

        Console.WriteLine(fX(GetIFromSteps(26501365L, firstIteration, gridSize)) == 608193767979991);
    }

    private long GetIFromSteps(long steps, long firstIteration, long otherIteration)
    {
        return (steps - firstIteration) / otherIteration;
    }

    private class Map20(
        string[] lines)
        : Map(lines, '.')
    {
        public Point[] GetAllPossibleMoves(Point location, bool infinite = false)
        {
            if (infinite == false)
                return new[]
                {
                    location + Point.ToUp,
                    location + Point.ToDown,
                    location + Point.ToLeft,
                    location + Point.ToRight
                }.Where(p => IsValidCoordinate(p) && GetValue(p) != '#').ToArray();

            return new[]
                {
                    location + Point.ToUp,
                    location + Point.ToDown,
                    location + Point.ToLeft,
                    location + Point.ToRight
                }.Select(m => new Point(m.X, m.Y))
                .Where(p => GetValue((int)(p.X % 131 + 131) % 131, (int)(p.Y % 131 + 131) % 131) != '#').ToArray();
        }
    }
}