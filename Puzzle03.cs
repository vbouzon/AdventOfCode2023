namespace AdventOfCode2023;

internal class Puzzle03
{
    public void Part01()
    {
        var map = new MapGame(File.ReadAllLines("Puzzle03.txt"));
        var coordinate = new Coordinate(0, 0);
        var result = 0;
        while (coordinate.IsEnd() == false)
        {
            var (number, nextCoordinate, range) = map.GetNumber(coordinate);
            if (number != -1)
            {
                var ok = range!
                    .SelectMany(map.GetNeighbours)
                    .Where(c => range!.Any(o => o.Equals(c)) == false)
                    .Distinct()
                    .Select(map.GetChar)
                    .Any(c => char.IsDigit(c) == false && c != '.');

                if (ok) result += number;
            }

            coordinate = nextCoordinate ?? coordinate.Next(map);
        }

        Console.WriteLine(result == 530849);
    }

    public void Part02()
    {
        var map = new MapGame(File.ReadAllLines("Puzzle03.txt"));
        var coordinate = new Coordinate(0, 0);
        var result = 0;

        while (coordinate.IsEnd() == false)
        {
            if (map.GetChar(coordinate) == '*')
            {
                var neighboursDigit = map.GetNeighbours(coordinate)
                    .Select(c => (Coordinate: c, Char: map.GetChar(c)))
                    .Where(c => char.IsDigit(c.Char))
                    .Select(c => map.GetNumber(c.Coordinate))
                    .Where(c => c.Number != -1 && c.Range != null)
                    .Select(c => (c.Number, Range: c.Range!.ToArray()))
                    .ToArray();

                var dis = neighboursDigit
                    .GroupBy(s => Coordinate.GetHashCode(s.Range))
                    .Select(g => g.First()).ToArray();

                if (dis.Length == 2) result += dis.Aggregate(1, (a, b) => a * b.Number);
            }

            coordinate = coordinate.Next(map);
        }

        Console.WriteLine(result == 84900879);
    }

    public readonly struct Coordinate(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;

        public Coordinate Next(MapGame map)
        {
            if (X == -1 || Y == -1) return new Coordinate(-1, -1);

            if (X + 1 == map.Map[Y].Length)
                return Y + 1 == map.Map.Length ? new Coordinate(-1, -1) : new Coordinate(0, Y + 1);

            return new Coordinate(X + 1, Y);
        }

        public bool IsValid(MapGame map)
        {
            return X >= 0 && Y >= 0 && Y < map.Map.Length && X < map.Map[Y].Length;
        }

        public bool IsEnd()
        {
            return Y == -1;
        }

        public override bool Equals(object? obj)
        {
            return obj is Coordinate coordinate &&
                   X == coordinate.X &&
                   Y == coordinate.Y;
        }

        public override int GetHashCode()
        {
            return X * 1000 + Y;
        }

        public static int GetHashCode(Coordinate[] a)
        {
            return a.OrderBy(c => c.X).ThenBy(c => c.Y).Sum(c => c.GetHashCode());
        }

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !(left == right);
        }
    }

    public class MapGame(string[] lines)
    {
        public char[][] Map { get; } = lines.Select(l => l.ToArray()).ToArray();

        public (int Number, Coordinate? Next, List<Coordinate>? Range) GetNumber(Coordinate coordinate)
        {
            var x = coordinate.X;
            var y = coordinate.Y;

            if (int.TryParse(Map[y][x].ToString(), out _) == false) return (-1, coordinate.Next(this), null);

            while (x >= 0)
            {
                if (int.TryParse(Map[y][x].ToString(), out _) == false) break;
                x--;
            }

            x++;
            var result = 0;

            List<Coordinate> coordinates = [];
            while (x < Map[y].Length)
            {
                if (int.TryParse(Map[y][x].ToString(), out var result1) == false) break;
                coordinates.Add(new Coordinate(x, y));
                result = result * 10 + result1;
                x++;
            }

            return (result, new Coordinate(x - 1, y).Next(this), coordinates);
        }


        public Coordinate[] GetNeighbours(Coordinate coordinate)
        {
            var x = coordinate.X;
            var y = coordinate.Y;
            return new[]
            {
                new Coordinate(coordinate.X - 1, coordinate.Y),
                new Coordinate(coordinate.X + 1, coordinate.Y),
                new Coordinate(coordinate.X, coordinate.Y - 1),
                new Coordinate(coordinate.X, coordinate.Y + 1),
                new Coordinate(x - 1, y - 1),
                new Coordinate(x + 1, y + 1),
                new Coordinate(x - 1, y + 1),
                new Coordinate(x + 1, y - 1)
            }.Where(c => c.IsValid(this)).ToArray();
        }

        public char GetChar(Coordinate coordinate)
        {
            var x = coordinate.X;
            var y = coordinate.Y;

            if (x < 0 || y < 0 || y >= Map.Length || x >= Map[y].Length) return ' ';

            return Map[y][x];
        }
    }
}