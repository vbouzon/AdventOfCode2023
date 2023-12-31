using System.Text.RegularExpressions;
using AdventOfCode2023.Utils;
using Microsoft.Z3;
using HailConfig = (long[], long[]);

namespace AdventOfCode2023;

public partial class Puzzle24
{
    public void Part01()
    {
        var puzzleInput = File.ReadAllText("Puzzle24.txt");
        var hailstones = ParseInput(puzzleInput).Select(h => new Hail(h)).ToArray();

        var pointDs = hailstones
            .SelectMany((h, i) =>
                hailstones.Skip(i + 1)
                    .Select(h2 => ((h, h2), LinearEquation.Intersection(h.GetEquation(), h2.GetEquation()))))
            .Where(p => p.Item2 != null
                        && p.Item1.h.GetT(p.Item2) >= 0
                        && p.Item1.h2.GetT(p.Item2) >= 0
                        && p.Item2.IsIn(
                            new PointD(200000000000000, 200000000000000),
                            new PointD(400000000000000, 400000000000000)));

        Console.WriteLine(pointDs.Count() == 24627);
    }

    public void Part02()
    {
        var puzzleInput = File.ReadAllText("Puzzle24.txt");
        var hails = ParseInput(puzzleInput).Select(h => new Hail(h)).ToArray();
        var result = EquationSolver(hails);

        Console.WriteLine(result == 527310134398221);
    }

    // A bit shitty, but it works
    private static long EquationSolver(Hail[] hails)
    {
        var ctx = new Context();
        var solver = ctx.MkSolver();

        var x = ctx.MkIntConst("x");
        var y = ctx.MkIntConst("y");
        var z = ctx.MkIntConst("z");
        var vx = ctx.MkIntConst("vx");
        var vy = ctx.MkIntConst("vy");
        var vz = ctx.MkIntConst("vz");

        for (var i = 0; i < 3; i++)
        {
            var hail = hails[i];
            var t = ctx.MkIntConst($"t{i}");

            var xSnowball = ctx.MkAdd(x, ctx.MkMul(t, vx));
            var ySnowball = ctx.MkAdd(y, ctx.MkMul(t, vy));
            var zSnowball = ctx.MkAdd(z, ctx.MkMul(t, vz));

            var xHail = ctx.MkAdd(ctx.MkInt(Convert.ToInt64(hail.Position.X)),
                ctx.MkMul(t, ctx.MkInt(Convert.ToInt64(hail.Velocity.X))));
            var yHail = ctx.MkAdd(ctx.MkInt(Convert.ToInt64(hail.Position.Y)),
                ctx.MkMul(t, ctx.MkInt(Convert.ToInt64(hail.Velocity.Y))));
            var zHail = ctx.MkAdd(ctx.MkInt(Convert.ToInt64(hail.Position.Z)),
                ctx.MkMul(t, ctx.MkInt(Convert.ToInt64(hail.Velocity.Z))));

            solver.Add(ctx.MkEq(xSnowball, xHail));
            solver.Add(ctx.MkEq(ySnowball, yHail));
            solver.Add(ctx.MkEq(zSnowball, zHail));
            solver.Add(t >= 0);
        }

        solver.Check();
        var model = solver.Model;

        var result = long.Parse(model.Eval(x).ToString())
                     + long.Parse(model.Eval(y).ToString())
                     + long.Parse(model.Eval(z).ToString());

        return result;
    }

    private static List<HailConfig> ParseInput(string inputString)
    {
        var lines = inputString.Trim().Split(Environment.NewLine).Where(s => s != "").ToArray();
        var hailstones = new List<HailConfig>();

        foreach (var line in lines)
        {
            var match = LineRegex().Match(line);
            long[] pos =
            {
                long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value), long.Parse(match.Groups[3].Value)
            };
            long[] vel =
            {
                long.Parse(match.Groups[4].Value), long.Parse(match.Groups[5].Value), long.Parse(match.Groups[6].Value)
            };
            hailstones.Add((pos, vel));
        }

        return hailstones;
    }

    [GeneratedRegex(@"(-?\d+),[ ]+(-?\d+),[ ]+(-?\d+) @[ ]+(-?\d+),[ ]+(-?\d+),[ ]+(-?\d+)")]
    private static partial Regex LineRegex();

    public record LinearEquation(decimal A, decimal B)
    {
        private PointD Compute(decimal x)
        {
            return new PointD(x, A * x + B);
        }

        public static PointD? Intersection(LinearEquation e1, LinearEquation e2)
        {
            if (e1.A == e2.A) return null;

            var x = (e2.B - e1.B) / (e1.A - e2.A);

            return e1.Compute(x);
        }
    }

    private static class MathHelper
    {
        public static LinearEquation GetFromTwoPoints(Point p1, Point p2)
        {
            var slope = (decimal)(p2.Y - p1.Y) / (p2.X - p1.X);
            var b = p1.Y - slope * p1.X;

            return new LinearEquation(slope, b);
        }
    }

    private class Hail(HailConfig config)
    {
        public Point3D Velocity => new(config.Item2[0], config.Item2[1], config.Item2[2]);
        public Point3D Position => new(config.Item1[0], config.Item1[1], config.Item1[2]);

        private Point3D Simulate(int n)
        {
            return new Point3D(Position.X + Velocity.X * n,
                Position.Y + Velocity.Y * n,
                Position.Z + Velocity.Z * n);
        }

        public decimal GetT(PointD point)
        {
            return (point.X - Position.X) / Velocity.X;
        }

        public LinearEquation GetEquation()
        {
            return MathHelper.GetFromTwoPoints(Simulate(0).Only2D(), Simulate(1).Only2D());
        }
    }
}