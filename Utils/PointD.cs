namespace AdventOfCode2023.Utils;

public record PointD(decimal X, decimal Y)
{
    public static PointD operator +(PointD point1, PointD point2)
    {
        return new PointD(point1.X + point2.X, point1.Y + point2.Y);
    }

    public static PointD operator -(PointD point1, PointD point2)
    {
        return new PointD(point1.X - point2.X, point1.Y - point2.Y);
    }

    public static PointD operator -(PointD point)
    {
        return new PointD(-point.X, -point.Y);
    }

    public static PointD operator *(PointD point, decimal multiplier)
    {
        return new PointD(point.X * multiplier, point.Y * multiplier);
    }

    public static PointD operator /(PointD point, decimal divider)
    {
        return new PointD(point.X / divider, point.Y / divider);
    }

    public bool IsIn(PointD p1, PointD p2)
    {
        var x1 = Math.Min(p1.X, p2.X);
        var y1 = Math.Min(p1.Y, p2.Y);

        var x2 = Math.Max(p1.X, p2.X);
        var y2 = Math.Max(p1.Y, p2.Y);

        return x1 <= X && X <= x2 && y1 <= Y && Y <= y2;
    }
}