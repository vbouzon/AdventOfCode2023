namespace AdventOfCode2023.Utils;

public record Point(long X, long Y)
{
    public static readonly Point ToUp = new(0, -1);
    public static readonly Point ToDown = new(0, 1);
    public static readonly Point ToLeft = new(-1, 0);
    public static readonly Point ToRight = new(1, 0);

    public static Point operator +(Point point1, Point point2)
    {
        return new Point(point1.X + point2.X, point1.Y + point2.Y);
    }

    public static Point operator -(Point point)
    {
        return new Point(-point.X, -point.Y);
    }

    public static Point operator *(Point point, long multiplier)
    {
        return new Point(point.X * multiplier, point.Y * multiplier);
    }

    public bool IsExclusivelyHorizontalMovement()
    {
        return X != 0 && Y == 0;
    }

    public bool IsExclusivelyVerticalMovement()
    {
        return X == 0 && Y != 0;
    }

    public Point TurnLeft()
    {
        return new Point(Y, -X);
    }

    public Point TurnRight()
    {
        return new Point(-Y, X);
    }

    public Point Normalize()
    {
        return new Point(X == 0 ? 0 : X / Math.Abs(X), Y == 0 ? 0 : Y / Math.Abs(Y));
    }
}