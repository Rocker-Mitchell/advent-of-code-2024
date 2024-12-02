using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day02;

/// <summary>
/// Solution code for Day 2: Red-Nosed Reports
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 2: Red-Nosed Reports"; }
    }

    /*
    public string InputFile
    {
        get
        {
            var fullName = GetType().Namespace ?? "";
            var dir = Path.Combine(fullName.Split('.')[1..]);
            return Path.Combine(dir, "input.test.txt");
        }
    }
    //*/

    private int[][] ParseReports(string input)
    {
        var lines = input.Split('\n');
        var reports = new List<int[]>(lines.Length);
        foreach (var line in lines)
        {
            var levels = line.Split().Select(n => int.Parse(n)).ToArray();
            reports.Add(levels);
        }

        return [.. reports];
    }

    /// <summary>
    /// Helper to build new array with an index removed.
    /// </summary>
    private int[] ReportWithIndexRemoved(int[] report, int index)
    {
        var modifiedReport = new List<int>(report);
        modifiedReport.RemoveAt(index);
        return [.. modifiedReport];
    }

    private bool IsSafeLevelSequence(int a, int b, bool expectAscending)
    {
        if ((a < b) != expectAscending)
        {
            return false;
        }

        var absDiff = Math.Abs(a - b);
        if (absDiff < 1 || absDiff > 3)
        {
            return false;
        }

        return true;
    }

    private bool IsSafeReport(int[] report, bool useTolerance = false)
    {
        bool expectAscending = report[0] < report[1];

        bool safe = true;

        for (int idx = 1; idx < report.Length; idx++)
        {
            var lastLevel = report[idx - 1];
            var level = report[idx];

            if (!IsSafeLevelSequence(lastLevel, level, expectAscending))
            {
                safe = false;
                break;
            }
        }

        if (!safe && useTolerance)
        {
            // NB I really don't know a valid way to anticipate what index
            //  could be removed, so just testing the removal of every index
            //  recursively w/o tolerance enabled
            for (int idx = 0; idx < report.Length; idx++)
            {
                if (IsSafeReport(ReportWithIndexRemoved(report, idx)))
                {
                    safe = true;
                    break;
                }
            }
        }

        return safe;
    }

    private int CountSafeReports(int[][] reports, bool useTolerance = false)
    {
        var countSafe = 0;
        foreach (var report in reports)
        {
            if (IsSafeReport(report, useTolerance))
            {
                countSafe++;
            }
        }

        return countSafe;
    }

    /// <summary>
    /// With a list of reports of levels, count the number of reports
    /// satisfying the safety conditions:
    /// <list type="bullet">
    /// <item>The sequence of numbers all ascend or descend.</item>
    /// <item>
    /// The absolute difference between numbers in sequence is in the inclusive
    /// range of 1 to 3.
    /// </item>
    /// </list>
    /// </summary>
    /// <returns>The number of safe reports in the input.</returns>
    public object PartOne(string input)
    {
        var reports = ParseReports(input);

        return CountSafeReports(reports);
    }

    /// <summary>
    /// With a list of reports of levels, count the number of reports
    /// satisfying the safety conditions of part one, with a tolerance added
    /// where if a single level was removed then the report would be safe.
    /// </summary>
    /// <returns>The number of safe reports in the input.</returns>
    public object PartTwo(string input)
    {
        var reports = ParseReports(input);

        return CountSafeReports(reports, true);
    }
}
