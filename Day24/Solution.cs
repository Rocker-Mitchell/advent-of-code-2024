using System.Text.RegularExpressions;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day24;

/// <summary>
/// Solution code for Day 24: Crossed Wires
/// </summary>
partial class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 24: Crossed Wires"; }
    }

    enum LogicGate
    {
        And,
        Or,
        Xor,
    }

    record struct GateConnections((string, string) Inputs, LogicGate Gate, string Output)
    {
        public readonly bool ContainsInput(string input) =>
            Inputs.Item1 == input || Inputs.Item2 == input;

        public readonly bool AnInputStartsWith(string start) =>
            Inputs.Item1.StartsWith(start) || Inputs.Item2.StartsWith(start);
    };

    [GeneratedRegex(@"(\S+) (AND|OR|XOR) (\S+) -> (\S+)")]
    private static partial Regex GateConnectionsRegex();

    readonly Dictionary<string, bool> _initialSystem = [];

    readonly List<GateConnections> _initialGates = [];

    void ParseSystem(string input)
    {
        _initialSystem.Clear();
        _initialGates.Clear();

        var parts = input.Split("\n\n");

        // parse initial wire values into system
        foreach (var line in parts[0].Split('\n'))
        {
            var wire = line.Split(": ");
            _initialSystem[wire[0]] = wire[1] == "1";
        }

        // parse gates
        foreach (var line in parts[1].Split('\n'))
        {
            var match = GateConnectionsRegex().Match(line);
            var inputs = (match.Groups[1].Value, match.Groups[3].Value);
            var gate = match.Groups[2].Value switch
            {
                "AND" => LogicGate.And,
                "OR" => LogicGate.Or,
                "XOR" => LogicGate.Xor,
                _ => throw new Exception($"Unknown gate type: {match.Groups[2].Value}."),
            };
            var output = match.Groups[4].Value;
            _initialGates.Add(new GateConnections(inputs, gate, output));
        }
    }

    Dictionary<string, bool> SimulateGates(IEnumerable<GateConnections> gates)
    {
#pragma warning disable IDE0028 // Simplify collection initialization
        Dictionary<string, bool> system = new(_initialSystem);
#pragma warning restore IDE0028 // Simplify collection initialization
        List<GateConnections> tempGates = [.. gates];

        while (tempGates.Count > 0)
        {
            try
            {
                // find a gate that has both inputs known in the system
                var gate = tempGates.First(g =>
                    system.ContainsKey(g.Inputs.Item1) && system.ContainsKey(g.Inputs.Item2)
                );

                // calculate the output of the gate, then set the output wire value
                bool outputResult = gate.Gate switch
                {
                    LogicGate.And => system[gate.Inputs.Item1] && system[gate.Inputs.Item2],
                    LogicGate.Or => system[gate.Inputs.Item1] || system[gate.Inputs.Item2],
                    LogicGate.Xor => system[gate.Inputs.Item1] ^ system[gate.Inputs.Item2],
                    _ => throw new Exception($"Unknown gate type: {gate.Gate}."),
                };
                system[gate.Output] = outputResult;

                // remove the gate from the list for next iteration
                tempGates.Remove(gate);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(
                    $"Failed to determine inputs to finish processing remaining {tempGates.Count} gate(s).",
                    nameof(gates),
                    e
                );
            }
        }

        return system;
    }

    static long GetSystemNumber(Dictionary<string, bool> system, string wireGroupPrefix = "z")
    {
        // collect all the wire values with names that start with 'z', in ascending order
        var zWires = system
            .Where(kvp => kvp.Key.StartsWith(wireGroupPrefix))
            .OrderBy(kvp => kvp.Key);

        // iterate over wires to build a number from the bits they represent
        long output = 0;
        long bitMask = 1;
        foreach (var wire in zWires)
        {
            if (wire.Value)
            {
                output |= bitMask;
            }
            bitMask <<= 1;
        }

        return output;
    }

    public object PartOne(string input)
    {
        ParseSystem(input);
        var system = SimulateGates(_initialGates);
        return GetSystemNumber(system);
    }

    string? FirstZDependent(string wire)
    {
        var dependents = _initialGates.Where(g => g.ContainsInput(wire));

        // return a dependent with a z wire output
        foreach (var dependent in dependents)
        {
            if (dependent.Output.StartsWith('z'))
            {
                return dependent.Output;
            }
        }

        // if no dependent output has a z wire, recurse to find a dependent that does
        foreach (var dependent in dependents)
        {
            var result = FirstZDependent(dependent.Output);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    List<string> FindGateOutputsSwappedFromRippleCarryAdder()
    {
        // from: https://www.reddit.com/r/adventofcode/comments/1hla5ql/2024_day_24_part_2_a_guide_on_the_idea_behind_the/
        /*
        If this system is supposed to add two numbers together, then the gate
        connections likely represent a ripple carry adder. An output bit should
        be computed from x XOR y XOR carryIn. A carryOut bit should be computed
        from (x AND y) OR (carryIn AND (x XOR y)).

        If we expect only outputs to have been swapped, then some gates should
        not satisfy the following conditions:
        * If the output is a z wire, then its gate should be an XOR gate. This
            doesn't apply to the last bit of z.
        * If the output is not a z wire and the inputs are not x or y wires,
            then the gate should be either an AND or OR gate.
        */
        var badZGates = _initialGates
            .Where(g => g.Output.StartsWith('z'))
            .OrderBy(g => g.Output)
            .SkipLast(1)
            .Where(g => g.Gate != LogicGate.Xor);
        var badNonZGates = _initialGates.Where(g =>
            !g.Output.StartsWith('z')
            && !g.AnInputStartsWith("x")
            && !g.AnInputStartsWith("y")
            && g.Gate != LogicGate.And
            && g.Gate != LogicGate.Or
        );

        /*
        Six of the eight swapped gates should be found, split evenly between
        badZGates and badNonZGates. The pairs of swaps should be between these
        two sets of gates. We want to iterate badNonZGates, find the z wire
        that gate should have depended on (easy to find the z wire depending on
        it, then assume decrementing the wire value will be the wire desired),
        find a match to a badZGate output, and swap the gate outputs.
        */
        GateConnections[] swappedGates = [.. _initialGates];
        foreach (var badNonZGate in badNonZGates)
        {
            var zDependent = FirstZDependent(badNonZGate.Output);
            if (zDependent == null)
            {
                continue;
            }
            var zPreceding = "z" + (int.Parse(zDependent[1..]) - 1).ToString("D2");
            var badZGate = badZGates.First(g => g.Output == zPreceding);
            swappedGates[_initialGates.IndexOf(badNonZGate)] = new GateConnections(
                badNonZGate.Inputs,
                badNonZGate.Gate,
                badZGate.Output
            );
            swappedGates[_initialGates.IndexOf(badZGate)] = new GateConnections(
                badZGate.Inputs,
                badZGate.Gate,
                badNonZGate.Output
            );
        }

        /*
        Simulating with the swapped gates so far should result in a system with
        a z number that is off by some 2^n where n is within the number of bits
        input; this would be from a carry bit being swapped. We can compare
        this z number to the intended value of x + y and XOR to find the
        different bits. The result should have a pattern that groups 0s
        together as the least significant bits, which can be counted to
        determine the nth full adder that is affected. This should lead to two
        gates involving x_n and y_n as inputs, one AND and one XOR; these
        should be swapped.
        */
        var system = SimulateGates(swappedGates);
        var zNumber = GetSystemNumber(system);
        var xNumber = GetSystemNumber(system, "x");
        var yNumber = GetSystemNumber(system, "y");
        var expectedZNumber = xNumber + yNumber;
        var diff = zNumber ^ expectedZNumber;
        var diffBits = Convert.ToString(diff, 2);
        var badCarryBit = diffBits.Length - 1 - diffBits.LastIndexOf('1');
        var badCarryGates = _initialGates.Where(g =>
            g.ContainsInput("x" + badCarryBit.ToString("D2"))
            && g.ContainsInput("y" + badCarryBit.ToString("D2"))
        );

        // collect the output wires that were determined swapped
        List<string> swappedGateOutputs = [];
        swappedGateOutputs.AddRange(badZGates.Select(g => g.Output));
        swappedGateOutputs.AddRange(badNonZGates.Select(g => g.Output));
        swappedGateOutputs.AddRange(badCarryGates.Select(g => g.Output));

        return swappedGateOutputs;
    }

    public object PartTwo(string input)
    {
        ParseSystem(input);

        var swappedGateOutputs = FindGateOutputsSwappedFromRippleCarryAdder();
        return string.Join(',', swappedGateOutputs.OrderBy(s => s));
    }
}
