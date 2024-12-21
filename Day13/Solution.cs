using System.Text.RegularExpressions;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day13;

/// <summary>
/// Solution code for Day 13: Claw Contraption
/// </summary>
partial class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 13: Claw Contraption"; }
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

    private record struct LongVector2(long X, long Y);

    private readonly record struct ClawMachine(
        LongVector2 AButton,
        LongVector2 BButton,
        LongVector2 Prize
    )
    {
        public bool IsSolution(long aPresses, long bPresses)
        {
            long finalX = AButton.X * aPresses + BButton.X * bPresses,
                finalY = AButton.Y * aPresses + BButton.Y * bPresses;
            return finalX == Prize.X && finalY == Prize.Y;
        }
    }

    private ClawMachine[] clawMachines = [];

    [GeneratedRegex(@"X\+(\d+), Y\+(\d+)")]
    private static partial Regex ButtonOffsetRegex();

    [GeneratedRegex(@"X=(\d+), Y=(\d+)")]
    private static partial Regex PrizePositionRegex();

    private void ParseClawMachines(string input, bool unitConversionFix = false)
    {
        ClawMachine ParseClawMachine(string block)
        {
            var lines = block.Split('\n');

            var matchAButton = ButtonOffsetRegex().Match(lines[0]);
            var aButtonX = int.Parse(matchAButton.Groups[1].Value);
            var aButtonY = int.Parse(matchAButton.Groups[2].Value);

            var matchBButton = ButtonOffsetRegex().Match(lines[1]);
            var bButtonX = int.Parse(matchBButton.Groups[1].Value);
            var bButtonY = int.Parse(matchBButton.Groups[2].Value);

            var matchPrize = PrizePositionRegex().Match(lines[2]);
            long prizeX = int.Parse(matchPrize.Groups[1].Value);
            long prizeY = int.Parse(matchPrize.Groups[2].Value);

            if (unitConversionFix)
            {
                prizeX += 10_000_000_000_000;
                prizeY += 10_000_000_000_000;
            }

            return new ClawMachine(
                new LongVector2(aButtonX, aButtonY),
                new LongVector2(bButtonX, bButtonY),
                new LongVector2(prizeX, prizeY)
            );
        }

        var clawMachinesQuery =
            from block in input.Split("\n\n")
            select ParseClawMachine(block);

        clawMachines = [.. clawMachinesQuery];
    }

    private static long TokensCost(long aPresses, long bPresses)
    {
        return aPresses * 3 + bPresses * 1;
    }

    private static (long aPresses, long bPresses)
    TrySolveLinearEquationByElimination(ClawMachine machine)
    {
        /*
        initial linear equations expressing the problem:
            a * a_x + b * b_x = p_x
            a * a_y + b * b_y = p_y
        multiply equations to match multipliers on a:
            a * a_x * a_y + b * b_x * a_y = p_x * a_y
            a * a_y * a_x + b * b_y * a_x = p_y * a_x
        combine equations by subtraction:
            b * (b_x * a_y - b_y * a_x) = p_x * a_y - p_y * a_x
            b = (p_x * a_y - p_y * a_x) / (b_x * a_y - b_y * a_x)
        plug in solved b for a:
            a = (p_x - b * b_x) / a_x
        */
        long bPresses = (
            machine.Prize.X * machine.AButton.Y
            - machine.Prize.Y * machine.AButton.X
        ) / (
            machine.BButton.X * machine.AButton.Y
            - machine.BButton.Y * machine.AButton.X
        );
        long aPresses = (
            machine.Prize.X - bPresses * machine.BButton.X
        ) / machine.AButton.X;

        return (aPresses, bPresses);
    }

    private (int solvableMachines, long totalTokens) CalculateTotalTokens()
    {
        int solvableMachines = 0;
        long totalTokens = 0;
        foreach (var machine in clawMachines)
        {
            var (aPresses, bPresses) =
                TrySolveLinearEquationByElimination(machine);

            if (machine.IsSolution(aPresses, bPresses))
            {
                solvableMachines++;
                totalTokens += TokensCost(aPresses, bPresses);
            }
        }

        return (solvableMachines, totalTokens);
    }

    public object PartOne(string input)
    {
        ParseClawMachines(input);

        var (solvableMachines, totalTokens) = CalculateTotalTokens();

        return $"solvable machines: {solvableMachines}, total tokens: {totalTokens}";
    }

    public object PartTwo(string input)
    {
        ParseClawMachines(input, true);

        var (solvableMachines, totalTokens) = CalculateTotalTokens();

        return $"solvable machines: {solvableMachines}, total tokens: {totalTokens}";
    }
}
