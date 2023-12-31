using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle25
{
    public void Part01()
    {
        (Node, Node) minimumCut;
        do
        {
            var nodes = ParseInput(File.ReadAllLines("Puzzle25.txt"));
            minimumCut = FindMinimumCut(nodes.ToDictionary(n => n.Name, n => n));
        } while (minimumCut.Item1.Connections.Count() != 3);

        Console.WriteLine(minimumCut.Item1.Count * minimumCut.Item2.Count == 548960);
    }

    private (Node, Node) FindMinimumCut(Dictionary<string, Node> nodes)
    {
        var random = new Random((int)DateTime.Now.Ticks);
        var i = 0;
        while (nodes.Count > 2)
        {
            var randomIndex = random.Next(nodes.Count);
            var node1 = nodes.Values.ElementAt(randomIndex);
            var otherNodeName = node1.Connections.ElementAt(random.Next(node1.Connections.Count));
            var node2 = nodes[otherNodeName];

            var mergedConnections = new List<string>(node1.Connections.Concat(node2.Connections));
            mergedConnections.RemoveAll(s => s == node1.Name);
            mergedConnections.RemoveAll(s => s == node2.Name);

            var mergedNode = new Node("Merged_" + i, node1.Position, mergedConnections,
                node1.Count + node2.Count);

            foreach (var mergedConnection in mergedConnections.Distinct())
            {
                var first = nodes[mergedConnection];
                while (first.Connections.Remove(node1.Name)) first.Connections.Add(mergedNode.Name);
                while (first.Connections.Remove(node2.Name)) first.Connections.Add(mergedNode.Name);
            }

            nodes.Remove(node1.Name);
            nodes.Remove(node2.Name);
            nodes.Add(mergedNode.Name, mergedNode);
            i++;
        }

        return (nodes.First().Value, nodes.Skip(1).First().Value);
    }

    private static List<Node> ParseInput(string[] lines)
    {
        var nodes = new Dictionary<string, Node>();

        foreach (var line in lines)
        {
            var parts = line.Split(':');
            var name = parts[0].Trim();
            HashSet<string> connections = [..parts[1].Trim().Split(' ').Select(s => s.Trim())];

            if (nodes.TryGetValue(name, out var node))
                foreach (var connection in connections)
                    node.Connections.Add(connection);
            else
                nodes.Add(name, new Node(name, new PointD(0m, 0m), connections.ToList(), 1));


            foreach (var connectionName in connections)
                if (nodes.ContainsKey(connectionName))
                    nodes[connectionName].Connections.Add(name);
                else
                    nodes[connectionName] = new Node(connectionName, new PointD(0m, 0m), [name], 1);
        }

        return nodes.Values.ToList();
    }

    private record Node(string Name, PointD Position, List<string> Connections, int Count);
}