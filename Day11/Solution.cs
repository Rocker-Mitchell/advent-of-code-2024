using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day11;

/// <summary>
/// Solution code for Day 11: Plutonian Pebbles
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 11: Plutonian Pebbles"; }
    }

    private int[] startingStones = [];

    private void ParseStones(string input)
    {
        var inputQuery =
            from word in input.Split()
            select int.Parse(word);
        startingStones = inputQuery.ToArray();
    }

    /// <summary>
    /// Calculate how a stone changes in a step.
    /// </summary>
    /// <param name="value">The value of the stone.</param>
    /// <returns>
    /// A tuple of new value(s) of stones. The second item will be null to
    /// represent the first item being the new stone value, otherwise both
    /// items are set to represent the stone splitting into two new stones.
    /// </returns>
    private static (ulong, ulong?) ChangeStone(ulong value)
    {
        if (value == 0)
            return (1, null);

        var digits = value.ToString();
        if (digits.Length % 2 == 0)
        {
            int midpoint = digits.Length / 2;
            var left = digits[..midpoint];
            var right = digits[midpoint..];
            return (ulong.Parse(left), ulong.Parse(right));
        }

        return (value * 2024, null);
    }

    /// <summary>
    /// Iterate a step of changes through a given list of stone values,
    /// modifying it in place.
    /// </summary>
    /// <param name="values">A list of stone values to modify.</param>
    private static void ChangeStones(List<ulong> values)
    {
        for (int idx = 0; idx < values.Count; idx++)
        {
            var result = ChangeStone(values[idx]);

            values[idx] = result.Item1;

            if (result.Item2.HasValue)
            {
                // fake an extra index step to insert the extra value then
                //  continue the loop after
                idx++;
                values.Insert(idx, result.Item2.Value);
            }
        }
    }

    public object PartOne(string input)
    {
        ParseStones(input);

        List<ulong> mutableStones = [.. startingStones.Select(v => (ulong)v)];

        foreach (var step in Enumerable.Range(1, 25))
        {
            ChangeStones(mutableStones);
        }

        return mutableStones.Count;
        // sub-second time to compute, leave as working version
    }

    private readonly
    Dictionary<(ulong value, uint steps), ulong> numberOfStonesGeneratedCache = [];

    /// <summary>
    /// A recursive cached function to compute the number of stones generated
    /// from an initial value and a number of steps to iterate.
    /// </summary>
    /// <param name="value">The initial stone value.</param>
    /// <param name="steps">The number of steps to change the stone.</param>
    /// <returns>
    /// The number of stones resulting from changing the initial value by the
    /// number of steps.
    /// </returns>
    private ulong NumberOfStonesGenerated(ulong value, uint steps)
    {
        var cacheKey = (value, steps);
        if (
            numberOfStonesGeneratedCache.TryGetValue(
                cacheKey,
                out var cacheResult
            )
        )
        {
            return cacheResult;
        }

        while (steps > 0)
        {
            var changeResult = ChangeStone(value);
            // prep for next step
            value = changeResult.Item1;
            steps--;

            if (steps > 0 && changeResult.Item2.HasValue)
            {
                // recursively compute the split values, return sum to exit
                //  loop early
                ulong result =
                    NumberOfStonesGenerated(changeResult.Item1, steps)
                    + NumberOfStonesGenerated(changeResult.Item2.Value, steps);
                numberOfStonesGeneratedCache.Add(cacheKey, result);
                return result;
            }

            if (steps == 0)
            {
                // finished steps; handle split at end for return
                ulong result = (ulong)(changeResult.Item2.HasValue ? 2 : 1);
                numberOfStonesGeneratedCache.Add(cacheKey, result);
                return result;
            }
        }

        // if steps was 0 or less, wouldn't ever split
        numberOfStonesGeneratedCache.Add(cacheKey, 1);
        return 1;
    }

    public object PartTwo(string input)
    {
        ParseStones(input);

        // ChangeStones() takes very long to run 75 times, trying to
        // improve performance with caching in NumberOfStonesGenerated()

        ulong total = 0;

        foreach (var stoneValue in startingStones)
        {
            total += NumberOfStonesGenerated((ulong)stoneValue, 75);
        }

        return total;
    }
}
