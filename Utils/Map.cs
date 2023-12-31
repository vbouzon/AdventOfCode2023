using System.Text;

namespace AdventOfCode2023.Utils;

public class Map
{
    private readonly char[,] _data;
    private readonly Func<char, char> _printer = c => c;

    public Map(long width, long height)
    {
        Width = width;
        Height = height;
        _data = new char[height, width];

        for (var i = 0; i < Height; i++)
        for (var j = 0; j < Width; j++)
            _data[i, j] = ' ';
    }

    public Map(string[] lines, char empty, Func<char, char>? converter = null, Func<char, char>? printer = null)
    {
        converter ??= c => c;
        if (printer != null) _printer = printer;

        lines = lines.Where(lines => !string.IsNullOrWhiteSpace(lines)).ToArray();

        Height = lines.Length;
        if (lines.Any(l => l.Length != lines[0].Length))
            throw new ArgumentException("All lines must have the same length.");
        Width = lines[0].Length;

        _data = new char[Height, Width];

        Enumerable.Range(0, lines.Length)
            .SelectMany(y => Enumerable.Range(0, lines[0].Length).Select(x => (x, y)))
            .ToList().ForEach(p => _data[p.y, p.x] = converter(lines[p.y][p.x] == empty ? ' ' : lines[p.y][p.x]));
    }

    public long Width { get; }
    public long Height { get; }


    private void SetValue(long x, long y, char value)
    {
        if (IsValidCoordinate(x, y))
            _data[y, x] = value;
        else
            throw new InvalidOperationException(
                $"Invalid coordinates: ({x}, {y}). Coordinates must be within the map boundaries.");
    }

    public void SetValue(Point point, char value)
    {
        SetValue(point.X, point.Y, value);
    }

    protected char GetValue(Point point)
    {
        return GetValue(point.X, point.Y);
    }

    public void SetValues(List<Point> points, char value)
    {
        foreach (var point in points) SetValue(point.X, point.Y, value);
    }

    protected char GetValue(long x, long y)
    {
        if (IsValidCoordinate(x, y)) return _data[y, x];

        throw new InvalidOperationException(
            $"Invalid coordinates: ({x}, {y}). Coordinates must be within the map boundaries.");
    }

    public List<Point> GetPoints(char value)
    {
        var points = new List<Point>();

        for (var y = 0; y < Height; y++)
        for (var x = 0; x < Width; x++)
            if (_data[y, x] == value)
                points.Add(new Point(x, y));

        return points;
    }


    protected bool IsValidCoordinate(Point point)
    {
        return IsValidCoordinate(point.X, point.Y);
    }

    protected bool IsValidCoordinate(long x, long y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public override string ToString()
    {
        var mapString = new StringBuilder();

        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++) mapString.Append(_printer(_data[i, j]));

            mapString.AppendLine();
        }

        return mapString.ToString();
    }
}