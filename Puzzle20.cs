using System.Numerics;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023;

public class Puzzle20
{
    public void Part01()
    {
        var board = new ElectronicBoard(File.ReadAllText("Puzzle20.txt"));
        var history = Enumerable.Range(0, 1000).SelectMany(_ => board.TriggerButton()).ToList();

        var countLow = history.Count(h => h.signal == Signal.Low);
        var countHigh = history.Count(h => h.signal == Signal.High);

        Console.WriteLine(countLow * countHigh == 949764474);
    }

    public void Part02()
    {
        var board = new ElectronicBoard(File.ReadAllText("Puzzle20.txt"));

        var conjunctionModuleForRx = board.Modules.Single(m => m.Value.Destinations.Contains("rx")).Value;
        var settled = board.Modules.Where(m => m.Value.Destinations.Contains(conjunctionModuleForRx.Name))
            .ToDictionary(i => i.Value, _ => 0);
        var index = 1;
        do
        {
            var triggerButton = board.TriggerButton();
            var valueTuples = triggerButton.Where(s => settled.ContainsKey(s.from)).GroupBy(s => s.from)
                .SelectMany(s => s);

            foreach (var tuple in valueTuples)
                if (settled[tuple.from] == 0 && tuple.signal == Signal.High)
                    settled[tuple.from] = index;

            if (settled.All(v => v.Value != 0))
                break;

            index++;
        } while (true);


        Console.WriteLine(Helpers.FindLCM(settled.Select(s => new BigInteger(s.Value)).ToArray()) == 243221023462303);
    }

    public abstract record Signal
    {
        public static readonly Signal High = new HighT();
        public static readonly Signal Low = new LowT();

        public static Signal operator !(Signal signal)
        {
            if (signal is HighT)
                return new LowT();

            return new HighT();
        }

        private record HighT : Signal
        {
            public override string ToString()
            {
                return "High";
            }
        }

        private record LowT : Signal
        {
            public override string ToString()
            {
                return "Low";
            }
        }
    }

    public abstract class Module(string name)
    {
        public string Name { get; } = name;
        public string[] Destinations { get; init; } = Array.Empty<string>();

        public virtual void Connect(Module module)
        {
        }

        public abstract Signal? Trigger(Module module, Signal signal);

        public override string ToString()
        {
            return Name;
        }

        public class FlipFlopModule(string name) : Module(name)
        {
            private Signal _lastOutput = Signal.Low;

            public override Signal? Trigger(Module module, Signal signal)
            {
                if (signal == Signal.High)
                    return null;

                _lastOutput = !_lastOutput;

                return _lastOutput;
            }
        }

        public class Conjunction(string name) : Module(name)
        {
            private readonly Dictionary<Module, Signal> _connectedModules = new();

            public override void Connect(Module module)
            {
                _connectedModules.Add(module, Signal.Low);
            }

            public override Signal Trigger(Module module, Signal signal)
            {
                if (_connectedModules.ContainsKey(module) == false)
                    throw new NotSupportedException("Not connected to this module");

                _connectedModules[module] = signal;

                var trigger = _connectedModules.All(m => m.Value == Signal.High) ? Signal.Low : Signal.High;


                return trigger;
            }
        }

        public class BroadcastModule(string name) : Module(name)
        {
            public override Signal Trigger(Module module, Signal signal)
            {
                return signal;
            }
        }

        public class OutputModule(string name) : Module(name)
        {
            public override Signal? Trigger(Module module, Signal signal)
            {
                return null;
            }
        }

        public class ButtonModule(string name) : Module(name)
        {
            public override void Connect(Module module)
            {
                throw new NotSupportedException("Nothing can't be connected");
            }

            public override Signal Trigger(Module module, Signal signal)
            {
                if (module != this)
                    throw new NotSupportedException();

                return signal;
            }
        }
    }

    private class ElectronicBoard
    {
        public ElectronicBoard(string content)
        {
            Modules = new Dictionary<string, Module>();

            var lines = content.Split(Environment.NewLine);

            foreach (var line in lines)
            {
                var strings = line.Split("->");
                var typeAndName = strings[0].Trim();

                var connections = strings[1].Trim().Split(',').Select(s => s.Trim()).ToArray();
                switch (typeAndName)
                {
                    case "button":
                        Modules.Add(typeAndName, new Module.ButtonModule(typeAndName) { Destinations = connections });
                        break;
                    case "broadcaster":
                        Modules.Add(typeAndName, new Module.BroadcastModule(typeAndName)
                        {
                            Destinations = connections
                        });
                        break;

                    default:
                        switch (typeAndName[0])
                        {
                            case '%':
                                Modules.Add(typeAndName.Substring(1),
                                    new Module.FlipFlopModule(typeAndName.Substring(1)) { Destinations = connections });
                                break;
                            case '&':
                                Modules.Add(typeAndName.Substring(1),
                                    new Module.Conjunction(typeAndName.Substring(1)) { Destinations = connections });
                                break;
                            default:
                                Modules.Add(typeAndName, new Module.OutputModule(typeAndName)
                                {
                                    Destinations = connections
                                });
                                break;
                        }

                        break;
                }
            }

            Modules.Add("button", new Module.ButtonModule("button")
            {
                Destinations = new[] { "broadcaster" }
            });

            foreach (var module in Modules.Values.ToList())
            foreach (var connectedName in module.Destinations)
                if (Modules.TryGetValue(connectedName, out var value))
                    value.Connect(module);
                else
                    Modules.Add(connectedName, new Module.OutputModule(connectedName));
        }

        public Dictionary<string, Module> Modules { get; }

        private IEnumerable<Module> GetDestinations(Module module)
        {
            return module.Destinations.Select(n => Modules[n]);
        }

        public List<(Module from, Module to, Signal signal)> TriggerButton()
        {
            Queue<(Module from, Module to, Signal signal)> signals = new();
            List<(Module from, Module to, Signal signal)> history = [];

            var button = Modules["button"];
            signals.Enqueue((button, button, Signal.Low));
            while (signals.Count != 0)
            {
                var signalToTransmit = signals.Dequeue();
                history.Add(signalToTransmit);

                var newSignal = signalToTransmit.to.Trigger(signalToTransmit.from, signalToTransmit.signal);
                if (newSignal == null)
                    continue;

                foreach (var to in GetDestinations(signalToTransmit.to))
                    signals.Enqueue((signalToTransmit.to, to, newSignal));
            }

            return history.Skip(1).ToList();
        }
    }
}