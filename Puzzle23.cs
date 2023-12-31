using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle23
{
    public void Part01()
    {
        var allLines = File.ReadAllLines("Puzzle23.txt");
        Map23 map = new(allLines);

        var begin = new Point(1, 0);
        var currentHike = new Hike(begin, 0, [begin]);

        Console.WriteLine(map.Move(currentHike, true) == 2278);
    }
    
    public void Part02()
    {
        var allLines = File.ReadAllLines("Puzzle23.txt");
        Map23 map = new(allLines);

        var begin = new Point(1, 0);

        var currentHike = new Hike(begin, 0, [begin]);

        Console.WriteLine(map.Move(currentHike, false) == 6734);
    }

    private void PrintMap(string[] lines, Hike hike)
    {
        var map23 = new Map23(lines);
        foreach (var point in hike.Path) map23.SetValue(point, 'O');

        map23.SetValue(hike.Location, 'X');
        Console.WriteLine(map23.ToString());
    }

    private record struct Hike(Point Location, long Steps, HashSet<Point> Path);

    private class Map23(string[] lines) : Map(lines, '.')
    {
        private readonly Dictionary<Point, (Point, long, HashSet<Point> Path)> _shortcuts = new();

        private List<Point> GetPotentialMove(Point location, Hike currentHike, bool useArrow)
        {
            List<Point> newLocations = (useArrow, GetValue(location)) switch
            {
                (true, '>') => [location + Point.ToRight],
                (true, '<') => [location + Point.ToLeft],
                (true, '^') => [location + Point.ToUp],
                (true, 'v') => [location + Point.ToDown],
                _ => [location + Point.ToRight, location + Point.ToLeft, location + Point.ToUp, location + Point.ToDown]
            };

            return newLocations.Where(p =>
                IsValidCoordinate(p) && GetValue(p) != '#' && currentHike.Path.Contains(p) == false).ToList();
        }

        public long Move(Hike currentHike, bool useArrow)
        {
            if (currentHike.Location == new Point(Width - 2, Height - 1)) return currentHike.Steps;

            if (_shortcuts.TryGetValue(currentHike.Location, out var shortcut))
                if (shortcut.Path.Contains(currentHike.Path.Reverse().Skip(1).Take(1).Single()) == false)
                {
                    var newLocation = shortcut.Item1;
                    var newSteps = currentHike.Steps + shortcut.Item2;
                    var newPath = currentHike.Path.Concat(shortcut.Path).Append(newLocation).ToHashSet();
                    return Move(new Hike(newLocation, newSteps, newPath), useArrow);
                }

            var potentialMove = GetPotentialMove(currentHike.Location, currentHike, useArrow);

            switch (potentialMove.Count)
            {
                case 0:
                    return 0;
                case 1:
                {
                    var corridorHike = new Hike(currentHike.Location, 0,
                        currentHike.Path.Reverse().Take(2).Reverse().ToHashSet());

                    if (GetPotentialMove(corridorHike.Location, corridorHike, useArrow).Count != 1)
                        return Move(new Hike(potentialMove.Single(), currentHike.Steps + 1,
                            currentHike.Path.Append(potentialMove.Single()).ToHashSet()), useArrow);

                    Hike? previousCorridorHike = null;
                    do
                    {
                        var move = GetPotentialMove(corridorHike.Location, corridorHike, useArrow);
                        if (move.Count == 1)
                        {
                            previousCorridorHike = corridorHike;
                            corridorHike = new Hike(move[0], corridorHike.Steps + 1,
                                corridorHike.Path.Append(move[0]).ToHashSet());
                        }
                        else
                        {
                            break;
                        }
                    } while (true);

                    if (previousCorridorHike != null && corridorHike.Steps > 1)
                    {
                        var startLocation = currentHike.Location;
                        var endLocation = previousCorridorHike.Value.Location;
                        var path = previousCorridorHike.Value.Path.Skip(1);
                        _shortcuts[startLocation] = (endLocation, previousCorridorHike.Value.Steps,
                            path.Take(2).Concat(path.Reverse().Take(2).Reverse()).ToHashSet());

                        path = path.Reverse();
                        _shortcuts[endLocation] = (startLocation, previousCorridorHike.Value.Steps,
                            path.Take(2).Concat(path.Reverse().Take(2).Reverse()).ToHashSet());
                    }

                    var newHike = new Hike(corridorHike.Location, currentHike.Steps + corridorHike.Steps,
                        currentHike.Path.Concat(corridorHike.Path.Take(2)
                                .Concat(corridorHike.Path.Reverse().Take(2).Reverse()))
                            .ToHashSet());

                    return Move(newHike, useArrow);
                }
                default:
                    return potentialMove.Select(m => Move(new Hike(m, currentHike.Steps + 1,
                        currentHike.Path.Append(m).ToHashSet()), useArrow)).Max();
            }
        }
    }
}