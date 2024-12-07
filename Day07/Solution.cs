using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day07;

/// <summary>
/// Solution code for Day 7: Bridge Repair
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 7: Bridge Repair"; }
    }

    private static (long value, long[] sequence)[] ParseTests(string input)
    {
        var lines = input.Split('\n');

        var tests = new List<(long value, long[] sequence)>(lines.Length);

        foreach (var line in lines)
        {
            var pair = line.Split(": ");

            long? value = null;
            try
            {
                value = long.Parse(pair[0]);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to parse test value: {pair[0]}", nameof(input), e);
            }

            long[] sequence = pair[1].Split().Select(s =>
            {
                try
                {
                    var v = long.Parse(s);
                    return v;
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Failed to parse sequence value: {s}", nameof(input), e);
                }
            }).ToArray();

            tests.Add((value.Value, sequence));
        }

        return [.. tests];
    }

    private static IEnumerable<long> ProducePossibleValues(
        long[] sequence,
        bool withConcat = false
    )
    {
        if (sequence.Length == 0)
            throw new ArgumentException("The sequence must be non-empty", nameof(sequence));

        if (sequence.Length == 1)
        {
            // recursive base state
            yield return sequence[0];
        }
        else
        {
            long extract = sequence[^1];
            long[] subsequence = sequence[..^1];

            // recursion
            var subValues = ProducePossibleValues(subsequence, withConcat);

            foreach (var value in subValues)
            {
                yield return value + extract;

                yield return value * extract;

                if (withConcat)
                    yield return long.Parse(value.ToString() + extract.ToString());
            }
        }
    }

    private static bool IsCalculableTest((long value, long[] sequence) test, bool withConcat = false)
    {
        foreach (var possible in ProducePossibleValues(test.sequence, withConcat))
        {
            if (possible == test.value)
                return true;
        }

        return false;
    }

    public object PartOne(string input)
    {
        var tests = ParseTests(input);

        long total = 0;
        foreach (var test in tests)
        {
            if (IsCalculableTest(test))
                total += test.value;
        }

        return total;
    }

    public object PartTwo(string input)
    {
        var tests = ParseTests(input);

        long total = 0;
        foreach (var test in tests)
        {
            if (IsCalculableTest(test, true))
                total += test.value;
        }

        return total;
    }
}
