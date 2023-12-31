using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle14
{
    public void Part01()
    {
        var lines = File.ReadAllLines("Puzzle14.txt");
        var map = new Map14(lines);


        var movement = Point.ToUp;
        MoveWhile(map, movement);

        Console.WriteLine(map.GetPoints('O').Sum(r => lines.Length - r.Y) == 110407);
    }

    public void Part_EdgeCase()
    {
        var lines = File.ReadAllLines("Puzzle14.txt");
        var map = new Map14(lines);

        var movement = Point.ToDown;
        var result = MoveWhile(map, movement);

        Console.WriteLine("Result " + result.iteration);
    }

    private static (long iteration, long movementCount) MoveWhile(Map14 map, Point movement)
    {
        int movementCount = 0, newMovementCount, iteration = 0;
        do
        {
            newMovementCount = map.Move('O', movement);
            movementCount += newMovementCount;

            iteration++;
        } while (newMovementCount > 0);

        return (iteration, movementCount);
    }

    public void Part02()
    {
        var lines = File.ReadAllLines("Puzzle14.txt");
        var map = new Map14(lines);


        var historyMap = new HashSet<string>();
        for (var i = 0; i < 1000000000; i++)
        {
            MoveWhile(map, Point.ToUp);
            MoveWhile(map, Point.ToLeft);
            MoveWhile(map, Point.ToDown);
            MoveWhile(map, Point.ToRight);

            var stringMap = map.ToString();
            if (historyMap.Add(stringMap)) continue;
            var found = historyMap.WithIndex().First(r => r.Item == stringMap);

            var nbSteps = (1000000000 - i) % (i - found.Index) - 1;

            var good =
                new Map(historyMap.ElementAt(found.Index + nbSteps).Split(Environment.NewLine), ' ').GetPoints('O');
            Console.WriteLine(good.Sum(r => lines.Length - r.Y) == 87273);
            return;
        }
    }

    private class Map14(string[] lines) : Map(lines, '.')
    {
        public int Move(char value, Point vector)
        {
            // if (vector.IsExclusivelyVerticalMovement() || vector.IsExclusivelyHorizontalMovement())
            // {
            //     return MoveMultiThread(value, vector);
            // }
            // else
            // {
            return MoveSingleThread(value, vector);
            // }
        }

        private int MoveMultiThread(char value, Point vector)
        {
            var movementCount = 0;
            (Func<Point, long>, Func<Point, long>, long) selector = vector.Normalize() switch
            {
                { } p when p == Point.ToUp => (r => r.X, r => r.Y, Height),
                { } p when p == Point.ToLeft => (r => r.Y, r => r.X, Width),
                { } p when p == Point.ToDown => (r => r.X, r => -r.Y, Height),
                { } p when p == Point.ToRight => (r => r.Y, r => -r.X, Width),
                _ => throw new ArgumentOutOfRangeException()
            };

            var pointsToMove = GetPoints(value);
            var groups = pointsToMove.GroupBy(i => selector.Item1(i)).ToDictionary(o => o.Key, o => o.ToArray());

            Enumerable.Range(0, (int)selector.Item3)
                .Select(i => groups.TryGetValue(i, out var group) ? group : Array.Empty<Point>())
                .AsParallel().ForAll(g =>
                {
                    foreach (var item in g.OrderBy(p => selector.Item2(p)))
                    {
                        var newPosition = item + vector;
                        if (IsValidCoordinate(newPosition.X, newPosition.Y) && GetValue(newPosition) == ' ')
                        {
                            SetValue(item, ' ');
                            SetValue(newPosition, value);

                            movementCount++;
                        }
                    }
                });

            return movementCount;
        }

        private int MoveSingleThread(char value, Point vector)
        {
            (Func<Point, long>, Func<Point, long>) selector = vector.Normalize() switch
            {
                { } p when p == Point.ToUp => (r => r.Y, _ => 1),
                { } p when p == Point.ToLeft => (r => r.X, _ => 1),
                { } p when p == Point.ToDown => (r => -r.Y, _ => 1),
                { } p when p == Point.ToRight => (r => -r.X, _ => 1),
                { } p when p.Y == Point.ToUp.Y && p.X == Point.ToLeft.X => (r => r.Y, r => r.X),
                { } p when p.Y == Point.ToDown.Y && p.X == Point.ToLeft.X => (r => -r.Y, r => r.X),
                { } p when p.Y == Point.ToUp.Y && p.X == Point.ToRight.X => (r => r.Y, r => -r.X),
                { } p when p.Y == Point.ToDown.Y && p.X == Point.ToRight.X => (r => -r.Y, r => -r.X),
                _ => throw new ArgumentOutOfRangeException()
            };

            var movementCount = 0;
            var pointsToMove = GetPoints(value).OrderBy(s => selector.Item1(s)).ThenBy(s => selector.Item2(s))
                .ToArray();

            foreach (var point in pointsToMove)
            {
                var newPosition = point + vector;

                if (!IsValidCoordinate(newPosition.X, newPosition.Y) || GetValue(newPosition) != ' ') continue;

                SetValue(point, ' ');
                SetValue(newPosition, value);

                movementCount++;
            }

            return movementCount;
        }
    }
}