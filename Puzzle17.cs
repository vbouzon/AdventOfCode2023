using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle17
{
    public void Part02()
    {
        var test = new Map17(File.ReadAllLines("Puzzle17.txt"));

        int move;
        do
        {
            move = test.Move(4, 10);
        } while (move == -1);

        Console.WriteLine(move == 925);
    }

    public void Part01()
    {
        var test = new Map17(File.ReadAllLines("Puzzle17.txt"));

        int move;
        do
        {
            move = test.Move(1, 3);
        } while (move == -1);

        Console.WriteLine(move == 817);
    }

    private record LocationAndMovement(Point Position, Point Direction, int Steps);

    private record State(LocationAndMovement Location, List<LocationAndMovement> History, int Weight)
    {
        public List<LocationAndMovement> GetHistoryWithCurrentBlock()
        {
            return History;
            //Disable for performance, useful for debugging
        }
        //return history.Append(ToVisited()).ToList();
    }

    private class Map17 : Map
    {
        private readonly Point _destination;
        private readonly Dictionary<LocationAndMovement, int> _globalHistory = new();
        private readonly PriorityQueue<State, int> _states = new();

        public Map17(string[] lines) : base(lines, '.', c => (char)int.Parse(c.ToString()),
            c => ((int)c).ToString()[0])

        {
            _states.Enqueue(new State(new LocationAndMovement(new Point(0, 0), Point.ToRight, 1), [], 0), 0);
            _states.Enqueue(new State(new LocationAndMovement(new Point(0, 0), Point.ToDown, 1), [], 0), 0);

            _destination = new Point(lines[0].Length - 1, lines.Length - 1);
        }

        public int Move(int minMove, int maxMove)
        {
            var currentVisitor = _states.Dequeue();
            if (currentVisitor.Location.Position == _destination)
                return currentVisitor.Weight;

            var newStates = GetStatesPlus1(currentVisitor, minMove, maxMove);
            foreach (var newState in newStates)
            {
                if (newState.Location.Position == _destination)
                    if (newState.Location.Steps < minMove)
                        continue;

                _states.Enqueue(newState, newState.Weight);
            }

            return -1;
        }

        private bool IsNotVisitedOrShorter(State potentialState)
        {
            var location = potentialState.Location;
            if (_globalHistory.TryGetValue(location, out var value))
            {
                // Given the graph, the first time we visit a node is the shortest path
                // So this check is useless, but it's a good check to have in general
                if (value <= potentialState.Weight) return false;

                Console.WriteLine($"Found shorter path: {potentialState.Weight} {location}");
                _globalHistory[location] = potentialState.Weight;
                return true;
            }

            _globalHistory.Add(location, potentialState.Weight);
            return true;
        }

        private IEnumerable<State> GetStatesPlus1(State state, int minMove, int maxMove)
        {
            var newHistory = state.GetHistoryWithCurrentBlock();
            var mustTurn = state.Location.Steps + 1 > maxMove;
            var canTurn = state.Location.Steps + 1 > minMove;

            if (mustTurn == false)
            {
                var enFace = state.Location.Position + state.Location.Direction;
                if (IsValidCoordinate(enFace))
                {
                    var item = new State(
                        new LocationAndMovement(enFace, state.Location.Direction, state.Location.Steps + 1),
                        newHistory, state.Weight + GetValue(enFace));

                    if (IsNotVisitedOrShorter(item))
                        yield return item;
                }
            }

            if (canTurn)
            {
                var turnLeft = state.Location.Position + state.Location.Direction.TurnLeft();
                if (IsValidCoordinate(turnLeft))
                {
                    var item = new State(
                        new LocationAndMovement(turnLeft, state.Location.Direction.TurnLeft(), 1),
                        newHistory, state.Weight + GetValue(turnLeft));

                    if (IsNotVisitedOrShorter(item))
                        yield return item;
                }

                var turnRight = state.Location.Position + state.Location.Direction.TurnRight();
                if (IsValidCoordinate(turnRight))
                {
                    var item = new State(
                        new LocationAndMovement(turnRight, state.Location.Direction.TurnRight(),
                            1), newHistory, state.Weight + GetValue(turnRight));

                    if (IsNotVisitedOrShorter(item))
                        yield return item;
                }
            }
        }


        public string DisplayStateWithHistory(State state)
        {
            var originalMap = ToString().Split(Environment.NewLine).ToArray();
            foreach (var block in state.GetHistoryWithCurrentBlock().Select(b => b.Position))
            {
                var movement = state.Location.Direction switch
                {
                    { } p when p == Point.ToDown => 'v',
                    { } p when p == Point.ToUp => '^',
                    { } p when p == Point.ToLeft => '<',
                    { } p when p == Point.ToRight => '>',
                    _ => throw new ArgumentOutOfRangeException()
                };

                var newString = originalMap[block.Y].ToCharArray();
                newString[block.X] = movement;
                originalMap[block.Y] = new string(newString);
            }

            return string.Join(Environment.NewLine, originalMap);
        }
    }
}