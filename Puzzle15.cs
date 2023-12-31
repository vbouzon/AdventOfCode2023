namespace AdventOfCode2023;

public class Puzzle15
{
    public void Part01()
    {
        var result = File.ReadAllText("Puzzle15.txt").Split(',').Select(Hash).Sum();
        Console.WriteLine(result == 504036);
    }

    public void Part02()
    {
        var lensList = Enumerable.Range(0, 256).Select(_ => new Box([])).ToArray();
        var aggregate = ParseSteps(File.ReadAllText("Puzzle15.txt")).Aggregate(lensList, ComputeNewBoxes);

        Console.WriteLine(GetPower(aggregate) == 295719);
    }

    private Box[] ComputeNewBoxes(Box[] boxes, Step step)
    {
        var box = boxes[Hash(step.Label)];
        var lens = box.Lenses.FindIndex(lens => lens.Label == step.Label);

        switch (step.FocalLength, lens)
        {
            case (null, >= 0):
                box.Lenses.RemoveAt(lens);
                break;
            case ({ } focal, >= 0):
                box.Lenses[lens] = new Lens(step.Label, focal);
                break;
            case ({ } focal, < 0):
                box.Lenses.Add(new Lens(step.Label, focal));
                break;
        }

        return boxes;
    }

    private IEnumerable<Step> ParseSteps(string input)
    {
        return input.Split(',')
            .Select(item => new { item, parts = item.Split('-', '=') })
            .Select(i => new Step(i.parts[0], i.parts[1] == "" ? null : int.Parse(i.parts[1])));
    }

    private int GetPower(Box[] boxes)
    {
        return Enumerable.Range(0, boxes.Length)
            .SelectMany(b => Enumerable.Range(0, boxes[b].Lenses.Count),
                (b, l) => (b + 1) * (l + 1) * boxes[b].Lenses[l].FocalLength).Sum();
    }

    private int Hash(string text)
    {
        return text.Aggregate(0, (ch, a) => (ch + a) * 17 % 256);
    }

    private record Lens(string Label, int FocalLength);

    private record Box(List<Lens> Lenses);

    private record Step(string Label, int? FocalLength);
}