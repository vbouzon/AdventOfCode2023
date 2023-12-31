using System.Collections.Concurrent;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle16
{
    public void Part01()
    {
        var lines = File.ReadAllLines("Puzzle16.txt");
        Console.WriteLine(SimulateBeam(lines, new PointAndDirection(new Point(-1, 0), Point.ToRight)) == 7798);
    }
    
    public void Part02()
    {
        var lines = File.ReadAllLines("Puzzle16.txt");

        ConcurrentBag<int> results = [];
        var map = new Map16(lines, new PointAndDirection(new Point(0, 0), new Point(0, 0)));

        foreach (var i in Enumerable.Range(0, (int)map.Width))
        {
            results.Add(SimulateBeam(lines, new PointAndDirection(new Point(i, -1), Point.ToDown)));
            results.Add(SimulateBeam(lines, new PointAndDirection(new Point(i, (int)map.Height), Point.ToUp)));
        }

        foreach (var i in Enumerable.Range(0, (int)map.Height))
        {
            results.Add(SimulateBeam(lines, new PointAndDirection(new Point(-1, i), Point.ToRight)));
            results.Add(SimulateBeam(lines, new PointAndDirection(new Point(map.Width, i), Point.ToLeft)));
        }

        Console.WriteLine(results.Max() == 8026);
    }

    private static int SimulateBeam(string[] lines, PointAndDirection init)
    {
        var traces = new HashSet<PointAndDirection>();
        var map = new Map16(lines, init);

        do
        {
            map.Move();

            var index = 0;
            foreach (var tuple in map.Beams.ToList())
            {
                if (tuple != null && !traces.Add(tuple))
                    map.Beams[index] = null;

                index++;
            }
        } while (map.Beams.Length != 0);

        return traces.Select(t => t.PointL).Distinct().Count();
    }

    private record PointAndDirection(Point PointL, Point Direction);

    private class Map16(string[] lines, PointAndDirection beam) : Map(lines, '.')
    {
        public PointAndDirection?[] Beams =
        {
            beam
        };

        private IEnumerable<PointAndDirection> Move(PointAndDirection point)
        {
            var (position, movement) = (point.PointL, point.Direction);
            var newPosition = position + movement;

            if (!IsValidCoordinate(newPosition.X, newPosition.Y)) return Array.Empty<PointAndDirection>();
            
            var value = GetValue(newPosition);
            var newDirections = value switch
            {
                '\\' when movement == Point.ToDown || movement == Point.ToUp => new[] { movement.TurnLeft() },
                '\\' when movement == Point.ToLeft || movement == Point.ToRight => new[] { movement.TurnRight() },

                '/' when movement == Point.ToDown || movement == Point.ToUp => new[] { movement.TurnRight() },
                '/' when movement == Point.ToLeft || movement == Point.ToRight => new[] { movement.TurnLeft() },

                '-' when movement == Point.ToRight || movement == Point.ToLeft => new[] { movement },
                '-' => new[] { Point.ToRight, Point.ToLeft },

                '|' when movement == Point.ToUp || movement == Point.ToDown => new[] { movement },
                '|' => new[] { Point.ToUp, Point.ToDown },

                ' ' => new[] { movement },

                _ => throw new InvalidOperationException("Invalid value")
            };

            return newDirections.Select(d => new PointAndDirection(newPosition, d));

        }

        public void Move()
        {
            Beams = Beams.Where(i => i != null).SelectMany(Move!).ToArray();
        }
    }
}