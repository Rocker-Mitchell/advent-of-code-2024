using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day25;

/// <summary>
/// The solution for Day 25: Code Chronicle
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 25: Code Chronicle"; }
    }

    static int[] CountFilledColumns(string schematicBlock)
    {
        var lines = schematicBlock.Split('\n');
        var columns = new int[lines[0].Length];
        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == '#')
                {
                    columns[j]++;
                }
            }
        }
        return columns;
    }

    readonly List<int[]> _locks = [];
    readonly List<int[]> _keys = [];
    int _maxHeight = 0;

    void ParseSchematics(string input)
    {
        _locks.Clear();
        _keys.Clear();

        var schematics = input.Split("\n\n");

        _maxHeight = schematics[0].Split('\n').Length;

        foreach (var schematic in schematics)
        {
            if (schematic[0] == '#')
            {
                _locks.Add(CountFilledColumns(schematic));
            }
            else
            {
                _keys.Add(CountFilledColumns(schematic));
            }
        }
    }

    bool LockOverlapsKey(int[] lockColumns, int[] keyColumns)
    {
        for (int i = 0; i < lockColumns.Length; i++)
        {
            if (lockColumns[i] + keyColumns[i] > _maxHeight)
            {
                return false;
            }
        }
        return true;
    }

    public object PartOne(string input)
    {
        ParseSchematics(input);

        int count = 0;
        for (int i = 0; i < _locks.Count; i++)
        {
            for (int j = 0; j < _keys.Count; j++)
            {
                if (LockOverlapsKey(_locks[i], _keys[j]))
                {
                    count++;
                }
            }
        }

        return count;
    }
}
