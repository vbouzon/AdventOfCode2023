namespace AdventOfCode2023.Utils;

public record AoCRange(decimal Start, decimal End)
{
    public static AoCRange FromDistance(decimal start, decimal distance)
    {
        return new AoCRange(start, start + distance);
    }


    public bool IsSubset(AoCRange other)
    {
        return other.Start >= Start && other.End <= End;
    }

    public bool Contains(decimal x)
    {
        return Start <= x && x <= End;
    }

    public bool IsOverlapping(AoCRange other)
    {
        return other.Start <= End && other.End >= Start;
    }

    public override string ToString()
    {
        return $"[{Start},{End}]";
    }

    public long Count()
    {
        return (long)(End - Start + 1);
    }

    public AoCRange[] Split(AoCRange other)
    {
        if (Equals(other, this))
            return [this];

        if (other.End <= Start || other.Start >= End)
            return [new AoCRange(Start, End)];

        if (other.Start <= Start && other.End >= End)
            return [this];

        if (other.Start == other.End)
            return [this with { End = other.Start }, other, this with { Start = other.End }];

        if (other.Start == Start && other.End < End)
            return [other, new AoCRange(other.End + 1, End)];

        if (other.Start > Start && other.End < End)
            return [this with { End = other.Start - 1 }, other, new AoCRange(other.End + 1, End)];

        if (other.Start > Start && other.End == End)
            return [this with { End = other.Start - 1 }, other];

        if (other.Start > Start && other.Start < End)
            return [this with { End = other.Start - 1 }, new AoCRange(other.Start, End)];

        if (other.End > Start && other.End < End)
            return [new AoCRange(Start, other.End), this with { Start = other.End + 1 }];

        throw new NotSupportedException();
    }
}