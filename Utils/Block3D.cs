namespace AdventOfCode2023.Utils;

public record Block3D(Point3D Begin, Point3D End)
{
    public long Elevation => Math.Min(Begin.Z, End.Z);

    public static Block3D FromString(string test)
    {
        var parts = test.Split('~');
        var begin = Point3D.FromString(parts[0]);
        var end = Point3D.FromString(parts[1]);
        return new Block3D(begin, end);
    }

    public bool Intersects(Block3D otherBlock)
    {
        var xIntersect = Math.Max(Begin.X, otherBlock.Begin.X) <=
                         Math.Min(End.X, otherBlock.End.X);

        var yIntersect = Math.Max(Begin.Y, otherBlock.Begin.Y) <=
                         Math.Min(End.Y, otherBlock.End.Y);

        var zIntersect = Math.Max(Begin.Z, otherBlock.Begin.Z) <=
                         Math.Min(End.Z, otherBlock.End.Z);

        return xIntersect && yIntersect && zIntersect;
    }

    public bool IsSupportedBy(Block3D otherBlock)
    {
        if (this == otherBlock)
            return false;

        return Elevation != 1 && ToDown().Intersects(otherBlock);
    }

    public bool IsSupporting(Block3D otherBlock)
    {
        return otherBlock.IsSupportedBy(this);
    }

    public Block3D ToDown()
    {
        return new Block3D(Begin + Point3D.ToDown, End + Point3D.ToDown);
    }

    public override string ToString()
    {
        return $"{Begin}~{End}";
    }
}