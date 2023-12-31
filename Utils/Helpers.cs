using System.Numerics;

namespace AdventOfCode2023.Utils;

public static class Helpers
{
    public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((item, index) => (item, index));
    }

    public static Func<long, long> FindQuadratic(List<long> results)
    {
        if (results.Count < 3) throw new ArgumentException("List must contain at least 3 elements");

        var c = results[0];
        var ab = results[1] - c;
        var fourATwoB = results[2] - c;
        var a = (fourATwoB - 2 * ab) / 2;
        var b = ab - a;

        return n => a * n * n + b * n + c;
    }

    public static BigInteger FindLCM(BigInteger[] numbers)
    {
        if (numbers.Length == 0) throw new ArgumentException("Array must contain at least one element");

        var lcm = numbers[0];

        for (var i = 1; i < numbers.Length; i++) lcm = FindLCM(lcm, numbers[i]);

        return lcm;
    }

    private static BigInteger FindLCM(BigInteger a, BigInteger b)
    {
        return a * b / FindGcd(a, b);
    }

    private static BigInteger FindGcd(BigInteger a, BigInteger b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }
}