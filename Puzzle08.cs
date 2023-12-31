using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public partial class Puzzle08
{
    public void Part01()
    {
        var lines = File.ReadAllLines("Puzzle08.txt");
        var instructions = lines[0];
        var nodes = lines.Skip(1)
            .Where(l => l.Length > 0)
            .Select(l => new
            {
                Line = l,
                Regex = Regex.Match(l, @"(\w+) = \((\w+), (\w+)\)")
            }).Select(x => new Node(
                x.Regex.Groups[1].Value,
                x.Regex.Groups[2].Value,
                x.Regex.Groups[3].Value
            )).ToDictionary(n => n.Id, n => n);

        var curNode = nodes["AAA"];
        var steps = 0;
        do
        {
            curNode = nodes[instructions[steps++ % instructions.Length] switch
            {
                'L' => curNode.Left,
                'R' => curNode.Right,
                _ => throw new ArgumentOutOfRangeException()
            }];
        } while (curNode.Id != "ZZZ");

        Console.WriteLine(steps == 12083);
    }

    public void Part02()
    {
        var lines = File.ReadAllLines("Puzzle08.txt");
        var instructions = lines[0];

        var nodes = lines.Skip(1)
            .Where(l => l.Length > 0)
            .Select(l => new
            {
                Line = l,
                Regex = LineRegex().Match(l)
            }).Select(x => new Node(
                x.Regex.Groups[1].Value,
                x.Regex.Groups[2].Value,
                x.Regex.Groups[3].Value
            )).ToDictionary(n => n.Id, n => n);

        var curNodes = nodes.Keys.Where(k => k.Last() == 'A').ToArray();
        var loops = new LoopInfo?[curNodes.Length];
        var paths = new List<string>[curNodes.Length];

        for (var i = 0; i < paths.Length; i++)
            paths[i] = [];

        while (true)
        {
            curNodes.Select((idOfCurrentNode, index) => (idOfCurrentNode, index))
                .AsParallel()
                .ForAll(item =>
                {
                    var stepIndex = paths[item.index].Count;
                    if (loops[item.index] != null)
                        return;

                    var curNode = item.idOfCurrentNode;

                    var idOfCurrentStep = stepIndex % instructions.Length + curNode;
                    paths[item.index].Add(idOfCurrentStep);

                    curNode = instructions[stepIndex % instructions.Length] switch
                    {
                        'L' => nodes[curNode].Left,
                        'R' => nodes[curNode].Right,
                        _ => curNode
                    };

                    var idOfNextStep = (stepIndex + 1) % instructions.Length + curNode;
                    if (paths[item.index].Contains(idOfNextStep)) loops[item.index] = new LoopInfo(paths[item.index]);

                    curNodes[item.index] = curNode;
                });

            if (loops.All(b => b != null))
                break;
        }


        var lcm = Helpers.FindLCM(loops.Select(loop => new BigInteger(loop!.WhereZ()[0])).ToArray());
        Console.WriteLine(lcm == 13385272668829);
    }

    [GeneratedRegex(@"(\w+) = \((\w+), (\w+)\)")]
    private static partial Regex LineRegex();

    private class LoopInfo(List<string> path)
    {
        public int[] WhereZ()
        {
            return path.WithIndex().Where(c => c.Item.Last() == 'Z').Select(c => c.Index).ToArray();
        }
    }

    private record Node(string Id, string Left, string Right);
}