using System.Text.RegularExpressions;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public partial class Puzzle19
{
    public enum Capacity
    {
        X,
        M,
        A,
        S
    }

    private (Workflow[], MachinePart[]) ParseInputs(string input)
    {
        var contentParts = input.Split(Environment.NewLine + Environment.NewLine);
        var workflows = contentParts[0].Split(Environment.NewLine).Select(ParseWorkflow).ToArray();
        var parts = contentParts[1].Split(Environment.NewLine).Select(ParsePart).ToArray();
        return (workflows, parts);
    }

    public void Part01()
    {
        var content = File.ReadAllText("Puzzle19.txt");
        var (workflows, parts) = ParseInputs(content);

        var approvedParts = new List<MachinePart>();

        foreach (var part in parts)
        {
            var workflow = workflows.First(w => w.Name == "in");
            while (true)
            {
                var decision = workflow.TakeDecision(part);
                switch (decision)
                {
                    case Decision.WorkflowReference workflowReference:
                        workflow = workflows.First(w => w.Name == workflowReference.WorkflowName);
                        continue;
                    case Decision.Approved:
                        approvedParts.Add(part);
                        break;
                }

                break;
            }
        }

        Console.WriteLine(approvedParts.Sum(p => p.GetSum()) == 342650);
    }


    private MachinePart ParsePart(string arg)
    {
        var regex = PartRegex();
        var matches = regex.Matches(arg);
        var dict = matches.Select(match =>
                KeyValuePair.Create(ParseCapacity(match.Groups[1].Value), long.Parse(match.Groups[2].Value)))
            .ToDictionary();

        return new MachinePart(dict);
    }

    private static Capacity ParseCapacity(string capacityStr)
    {
        return capacityStr switch
        {
            "x" => Capacity.X,
            "m" => Capacity.M,
            "a" => Capacity.A,
            "s" => Capacity.S,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Workflow ParseWorkflow(string arg1, int arg2)
    {
        var components = arg1.Split("{");
        var ruleStrings = components[1].Trim('}').Split(",");
        var rules = ruleStrings.Reverse().Skip(1).Reverse().Select(ParseRule).ToArray();
        var defaultDecision = ParseDecision(ruleStrings.Last());
        return new Workflow(components[0].Trim(), rules, defaultDecision);
    }

    private WorkflowRule ParseRule(string arg1, int arg2)
    {
        var strings = arg1.Split(":");
        var decisionString = strings[1];
        var decision = ParseDecision(decisionString);

        if (strings[0].Contains('>'))
        {
            var split = strings[0].Split(">");
            var capacity = ParseCapacity(split[0].Trim());
            var range = new AoCRange(decimal.Parse(split[1].Trim()) + 1, decimal.MaxValue);
            return new WorkflowRule(capacity, range, decision);
        }
        else
        {
            var split = strings[0].Split("<");
            var capacity = ParseCapacity(split[0].Trim());
            var range = new AoCRange(0, decimal.Parse(split[1].Trim()) - 1);
            return new WorkflowRule(capacity, range, decision);
        }
    }

    private static Decision ParseDecision(string str)
    {
        return str switch
        {
            "A" => new Decision.Approved(),
            "R" => new Decision.Rejected(),
            _ => new Decision.WorkflowReference(str)
        };
    }

    public void Part02()
    {
        var content = File.ReadAllText("Puzzle19.txt");
        var (workflows, _) = ParseInputs(content);

        var init = new MachinePartRange(new Dictionary<Capacity, AoCRange>
        {
            { Capacity.X, new AoCRange(1, 4000) },
            { Capacity.M, new AoCRange(1, 4000) },
            { Capacity.A, new AoCRange(1, 4000) },
            { Capacity.S, new AoCRange(1, 4000) }
        });

        var queue = new Queue<(MachinePartRange, Decision.WorkflowReference)>();
        var approved = new List<MachinePartRange>();

        queue.Enqueue((init, new Decision.WorkflowReference("in")));

        while (queue.Count != 0)
        {
            var current = queue.Dequeue();
            foreach (var (decision, newPart) in workflows.First(w => w.Name == current.Item2.WorkflowName)
                         .ForecastDecision(current.Item1))
                switch (decision)
                {
                    case Decision.Approved:
                        approved.Add(newPart);
                        break;
                    case Decision.WorkflowReference workflowReference:
                        queue.Enqueue((newPart, workflowReference));
                        break;
                }
        }

        Console.WriteLine(approved.Sum(p => p.GetSum()) == 130303473508222);
    }

    [GeneratedRegex(@"(\w)=(\d+)")]
    private static partial Regex PartRegex();

    public abstract record Decision
    {
        public record Approved : Decision
        {
            public override string ToString()
            {
                return "Approved";
            }
        }

        public record Rejected : Decision
        {
            public override string ToString()
            {
                return "Rejected";
            }
        }

        public record WorkflowReference(string WorkflowName) : Decision
        {
            public override string ToString()
            {
                return "Go to " + WorkflowName;
            }
        }
    }

    private record MachinePart(Dictionary<Capacity, long> Capacities)
    {
        public long GetSum()
        {
            return Capacities[Capacity.X] + Capacities[Capacity.M]
                                          + Capacities[Capacity.A] + Capacities[Capacity.S];
        }
    }

    public record MachinePartRange(Dictionary<Capacity, AoCRange> Capacities)
    {
        public long GetSum()
        {
            return Capacities[Capacity.X].Count() * Capacities[Capacity.M].Count() * Capacities[Capacity.A].Count() *
                   Capacities[Capacity.S].Count();
        }

        public MachinePartRange With(Capacity workflowRuleCapacity, AoCRange range)
        {
            return new MachinePartRange(new Dictionary<Capacity, AoCRange>(Capacities)
            {
                [workflowRuleCapacity] = range
            });
        }

        public override string ToString()
        {
            return "[" + string.Join(",", Capacities.Select(c => $"{c.Key}={c.Value}")) + "]";
        }
    }

    private record WorkflowRule(Capacity Capacity, AoCRange AoCRange, Decision Decision)
    {
        public override string ToString()
        {
            return Capacity + ",[" + AoCRange + "]=>" + Decision;
        }
    }

    private record Workflow(string Name, WorkflowRule[] Rules, Decision DefaultDecision)
    {
        public Decision TakeDecision(MachinePart part)
        {
            foreach (var workflowRule in Rules)
                if (part.Capacities.TryGetValue(workflowRule.Capacity, out var value))
                    if (workflowRule.AoCRange.Contains(value))
                        return workflowRule.Decision;

            return DefaultDecision;
        }

        public IEnumerable<(Decision, MachinePartRange)> ForecastDecision(MachinePartRange part)
        {
            var currentPart = part;

            foreach (var workflowRule in Rules)
                // Console.WriteLine($"Evaluating {workflowRule} for {currentPart})");
                if (currentPart.Capacities.TryGetValue(workflowRule.Capacity, out var currentCapacity))
                {
                    if (currentCapacity.IsOverlapping(currentPart.Capacities[workflowRule.Capacity]) == false)
                        // Console.WriteLine("This rule is not applicable, we ignore");
                        continue;

                    var newRanges = currentCapacity.Split(workflowRule.AoCRange);
                    switch (newRanges.Length)
                    {
                        case 1:
                            // Console.WriteLine("This rule is applicable on the whole range, we apply it");
                            // Console.WriteLine($"We apply {workflowRule.Decision} on {currentPart}");
                            yield return (workflowRule.Decision, currentPart);
                            yield break;
                        case 2:
                        {
                            // Console.WriteLine("This rule is applicable on a part of the range, we split it");
                            var subSetRange = newRanges.Single(s => workflowRule.AoCRange.IsSubset(s));
                            var applicablePart = currentPart.With(workflowRule.Capacity, subSetRange);
                            // Console.WriteLine($"We apply {workflowRule.Decision} on {applicablePart}");
                            yield return (workflowRule.Decision, applicablePart);

                            currentPart = currentPart.With(workflowRule.Capacity,
                                newRanges.Except(new[] { subSetRange }).Single());
                            // Console.WriteLine($"We continue with {currentPart}");
                            break;
                        }
                        default:
                            throw new NotSupportedException();
                    }
                }

            // Console.WriteLine($"We apply the default decision {DefaultDecision} on {currentPart}");
            yield return (DefaultDecision, currentPart);
        }
    }
}