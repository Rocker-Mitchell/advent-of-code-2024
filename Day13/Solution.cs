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

    private record struct DoubleVector2(double X, double Y);

    private readonly record struct ClawMachine(
        DoubleVector2 AButton,
        DoubleVector2 BButton,
        DoubleVector2 Prize
    )
    {
        public bool IsSolution(double aPresses, double bPresses)
        {
            double finalX = AButton.X * aPresses + BButton.X * bPresses,
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
            double prizeX = int.Parse(matchPrize.Groups[1].Value);
            double prizeY = int.Parse(matchPrize.Groups[2].Value);

            if (unitConversionFix)
            {
                prizeX += 1e13;
                prizeY += 1e13;
            }

            return new ClawMachine(
                new DoubleVector2(aButtonX, aButtonY),
                new DoubleVector2(bButtonX, bButtonY),
                new DoubleVector2(prizeX, prizeY)
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

    private static
    (double aPresses, double bPresses)
    SolveButtonPressIntersect(ClawMachine machine)
    {
        // initial linear formulas expressing the problem:
        //  a * a_x + b * b_x = p_x
        //  a * a_y + b * b_y = p_y
        // solving for b in first formula:
        //  b = (p_x - a*a_x) / b_x = - a * a_x/b_x + p_x/b_x
        // solving for a in second formula, plugging in solution for b
        //  p_y = a * (a_y - b_y * a_x/b_x) + b_y * p_x/b_x
        //  a = (p_y - b_y * p_x/b_x) / (a_y - b_y * a_x/b_x)
        double aPresses = (
            machine.Prize.Y
            - (
                machine.BButton.Y
                * machine.Prize.X
                / machine.BButton.X
            )
        ) / (
            machine.AButton.Y
            - (
                machine.BButton.Y
                * machine.AButton.X
                / machine.BButton.X
            )
        );
        double bPresses = (
            machine.Prize.X - (machine.AButton.X * aPresses)
        ) / machine.BButton.X;

        return (aPresses, bPresses);
    }

    private static
    double CoerceToNearestWholeNumber(double value, double marginOfError = 1e-6)
    {
        if (Math.Abs(value - Math.Round(value)) < marginOfError)
        {
            return Math.Round(value);
        }
        else
        {
            return value;
        }
    }

    private (int solvableMachines, long totalTokens) CalculateTotalTokens()
    {
        int solvableMachines = 0;
        long totalTokens = 0;
        foreach (var machine in clawMachines)
        {
            var (aPresses, bPresses) = SolveButtonPressIntersect(machine);

            // trying to address floating point errors
            aPresses = CoerceToNearestWholeNumber(aPresses);
            bPresses = CoerceToNearestWholeNumber(bPresses);

            if (
                aPresses >= 0 && bPresses >= 0
                && aPresses % 1 == 0 && bPresses % 1 == 0
            )
            {
                solvableMachines++;
                totalTokens += TokensCost((long)aPresses, (long)bPresses);
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
        // BUG getting too few tokens
        // TODO probably need an entirely different solution to solve
        //  intersection that won't fail from floating point precision errors,
        //  since this part clearly is targeting the use of floating point
    }
}
