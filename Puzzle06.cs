using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public class Puzzle06
{
    public void Part01()
    {
        var lines = File.ReadAllLines("Puzzle06.txt");

        var array = lines
            .Select(l => Regex.Matches(l, @"(\d+)").Select(m => BigInteger.Parse(m.Groups[1].Value)).ToArray())
            .ToArray();

        var races = array[0].Zip(array[1])
            .Select((o, _) => new Race(o.First, o.Second)).ToArray();

        var aggregate = races.Aggregate(new BigInteger(1), (i, r) => i * r.Possibilities());
        Console.WriteLine(aggregate == 625968);
    }

    public void Part02()
    {
        var lines = File.ReadAllLines("Puzzle06.txt");
        var o = lines.Select(l => Regex.Matches(l, @"(\d+)").Select(m => m.Value).Aggregate("", (a, b) => a + b))
            .ToArray();
        var race = new Race(BigInteger.Parse(o[0]), BigInteger.Parse(o[1]));

        Console.WriteLine(race.Possibilities() == 43663323);
    }

    public void Part_EdgeCase()
    {
        var race = new Race(5391676, 25013301081102);
        Console.WriteLine(race.Possibilities() == 43663323);
    }

    private record Race(BigInteger TotalTime, BigInteger Record)
    {
        private static BigInteger Speed(BigInteger push)
        {
            return push;
        }

        private static BigInteger Distance(BigInteger speed, BigInteger time)
        {
            return speed * time;
        }

        private BigInteger FindExactLowerBound()
        {
            var r = FindLowerBound(TotalTime / 2);

            if (GetPossibleDistance(r - 1) > Record) return r - 1;
            if (GetPossibleDistance(r) <= Record)
                return r + 1;

            return r;
        }

        private BigInteger FindLowerBound(BigInteger push)
        {
            var step = push / 2;
            do
            {
                if (GetPossibleDistance(push) > Record)
                {
                    if (GetPossibleDistance(push - 1) <= Record) return push;

                    push -= step;
                    step /= 2;
                    step = step == 0 ? 1 : step;
                    continue;
                }

                push += step;
                step /= 2;
                step = step == 0 ? 1 : step;
            } while (true);
        }

        private BigInteger GetPossibleDistance(BigInteger push)
        {
            var distance = Distance(Speed(push), TotalTime - push);
            if (distance < 0)
                throw new NotSupportedException();

            return distance;
        }

        public BigInteger Possibilities()
        {
            var lower = FindExactLowerBound();
            var upper = TotalTime - lower;
            return upper - lower + 1;
        }
    }
}