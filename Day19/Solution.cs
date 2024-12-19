using System.Collections.Concurrent;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day19;

class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 19: Linen Layout"; }
    }

    /*
    public string InputFile
    {
        get
        {
            var dir = NamespacePath.GetFolderPathFromType(GetType());
            return Path.Combine(dir, "input.test.txt");
        }
    }
    //*/

    HashSet<string> allTowels = [];
    string[] designs = [];

    void ParseInput(string input)
    {
        var parts = input.Split("\n\n");

        allTowels = [.. parts[0].Split(", ")];

        designs = parts[1].Split('\n');
    }

    class MemoizedMatchTowels(IEnumerable<string> towels)
    {
        readonly HashSet<string> towels = [.. towels];

        readonly ConcurrentDictionary<string, bool> canMatchMemo = [];
        public bool CanMatchTowels(string design)
        {
            return canMatchMemo.GetOrAdd(design, (design) =>
            {
                // base case
                if (design.Length == 0)
                    return true;

                foreach (var pattern in towels)
                {
                    if (
                        design.StartsWith(pattern)
                        && CanMatchTowels(design[pattern.Length..])
                    )
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        readonly ConcurrentDictionary<string, long> countMemo = [];
        public long CountUniqueMatchTowels(string design)
        {
            return countMemo.GetOrAdd(design, (design) =>
            {
                // base case
                if (design.Length == 0)
                    return 1;

                // initialize with no count
                long total = 0;

                foreach (var pattern in towels)
                {
                    if (design.StartsWith(pattern))
                    {
                        total += CountUniqueMatchTowels(
                            design[pattern.Length..]
                        );
                    }
                }

                return total;
            });
        }
    }

    public object PartOne(string input)
    {
        ParseInput(input);

        var memoizedMatchTowels = new MemoizedMatchTowels(allTowels);

        int total = 0;
        foreach (var design in designs)
        {
            if (memoizedMatchTowels.CanMatchTowels(design))
            {
                total++;
            }
        }

        return total;
    }

    public object PartTwo(string input)
    {
        ParseInput(input);

        var memoizedMatchTowels = new MemoizedMatchTowels(allTowels);

        long total = 0;
        foreach (var design in designs)
        {
            total += memoizedMatchTowels.CountUniqueMatchTowels(design);
        }

        return total;
    }
}
