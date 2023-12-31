namespace AdventOfCode2023.Utils;

public readonly record struct Point3D(long X, long Y, long Z)
{
    public static readonly Point3D ToDown = new(0, 0, -1);

    public static Point3D operator +(Point3D p1, Point3D p2)
    {
        return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public static Point3D FromString(string p0)
    {
        var parts = p0.Split(',');
        return new Point3D(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }

    public Point Only2D()
    {
        return new Point(X, Y);
    }
}