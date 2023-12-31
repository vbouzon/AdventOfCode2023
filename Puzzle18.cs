using System.Globalization;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle18
{
    public void Part01()
    {
        var lines = File.ReadAllLines("Puzzle18.txt");

        var instructions =
            lines.Select(l => l.Split(' '))
                .Select(parts => new Instruction(parts[0], parts[1])).ToArray();

        Console.WriteLine(Go(instructions) == 49061);
    }

    public void Part02()
    {
        var lines = File.ReadAllLines("Puzzle18.txt");

        var instructions =
            lines.Select(l => l.Split(' '))
                .Select(l => l[2].Trim('(', ')', '#'))
                .Select(l => new
                {
                    direction = l.Last() switch
                    {
                        '0' => "R",
                        '1' => "D",
                        '2' => "L",
                        '3' => "U",
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    distance = int.Parse(l[..^1], NumberStyles.HexNumber)
                })
                .Select(parts =>
                    new Instruction(parts.direction, parts.distance.ToString()))
                .ToArray();

        Console.WriteLine(Go(instructions) == 92556825427032);
    }


    private static long Go(Instruction[] instructions)
    {
        var vertices = instructions.Aggregate(new Point[] { new(0, 0) },
            (ls, instruction) => [..ls, ls.Last() + GetDirection(instruction) * int.Parse(instruction.Steps)]);

        return (long)Area(vertices);
    }

    private static Point GetDirection(Instruction instruction)
    {
        var direction = instruction.Direction switch
        {
            "R" => Point.ToRight,
            "L" => Point.ToLeft,
            "U" => Point.ToUp,
            "D" => Point.ToDown,
            _ => throw new ArgumentOutOfRangeException()
        };
        return direction;
    }

    private static double Area(Point[] polygon)
    {
        // E.g
        // ### 
        // ###
        // With polygon formula, this would be (0,0), (2,0), (2,1), (0,1) = 2
        // But we want 6, so we add the top, right border 6/2 + 1 = 4, 4 + 2 = 6
        // https://en.wikipedia.org/wiki/Shoelace_formula
        // https://en.wikipedia.org/wiki/Pick%27s_theorem
        var verticesCount = polygon.Length;
        var content = 0.0;
        var perimeter = 0.0;
        for (var i = 0; i < verticesCount - 1; i++)
        {
            content += polygon[i].Y * polygon[i + 1].X - polygon[i + 1].Y * polygon[i].X;
            perimeter += Math.Abs(polygon[i + 1].X - polygon[i].X) + Math.Abs(polygon[i + 1].Y - polygon[i].Y);
        }

        content = Math.Abs(content + polygon[verticesCount - 1].Y * polygon[0].X -
                           polygon[0].Y * polygon[verticesCount - 1].X) / 2.0;

        perimeter += Math.Abs(polygon[0].X - polygon[verticesCount - 1].X) +
                     Math.Abs(polygon[0].Y - polygon[verticesCount - 1].Y);

        return content + perimeter / 2 + 1;
    }

    private record Instruction(string Direction, string Steps);
}