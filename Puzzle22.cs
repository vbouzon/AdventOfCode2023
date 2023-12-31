using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle22
{
    public void Part01()
    {
        var allBlocks = File.ReadAllLines("Puzzle22.txt").Select(Block3D.FromString).ToList();
        allBlocks = Stabilize(allBlocks);

        var i = allBlocks.OrderByDescending(b => Math.Max(b.Begin.Z, b.End.Z))
            .AsParallel()
            .Count(currentBlock => allBlocks
                .Where(b => b.IsSupportedBy(currentBlock))
                .All(b => allBlocks.Except(new[] { currentBlock }).Any(b2 => b2.IsSupporting(b))));

        // Alternative solution (slower)
        // var i = allBlocks
        //     .AsParallel().Count(b => Map3D.MoveDown(allBlocks.Except(new[] { b })).CountMove == 0);

        Console.WriteLine(i == 465);
    }

    public void Part02()
    {
        var allBlocks = File.ReadAllLines("Puzzle22.txt").Select(Block3D.FromString).ToList();
        allBlocks = Stabilize(allBlocks);

        var i = allBlocks
            .AsParallel()
            .Sum(b => Map3D.MoveDown(allBlocks.Except(new[] { b })).CountMove);

        Console.WriteLine(i == 79042);
    }

    private static List<Block3D> Stabilize(List<Block3D> allBlocks)
    {
        var noMoveCount = 0;
        do
        {
            var (hasMoved, newBlocks) = Map3D.MoveDown(allBlocks);
            if (hasMoved == 0)
                noMoveCount++;

            allBlocks = newBlocks;
        } while (noMoveCount == 0);

        return allBlocks;
    }


    private static class Map3D
    {
        public static (int CountMove, List<Block3D> block3Ds) MoveDown(IEnumerable<Block3D> blocks)
        {
            var hasMoved = 0;
            var newBlocks = new List<Block3D>();
            foreach (var block in blocks.OrderBy(b => Math.Min(b.Begin.Z, b.End.Z)))
            {
                if (block.Elevation == 1)
                {
                    newBlocks.Add(block);
                    continue;
                }

                var newBlock = block.ToDown();
                if (newBlocks.Any(n => n.Intersects(newBlock)))
                {
                    newBlocks.Add(block);
                }
                else
                {
                    newBlocks.Add(newBlock);
                    hasMoved++;
                }
            }

            return (hasMoved, newBlocks);
        }
    }
}