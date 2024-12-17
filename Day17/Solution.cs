using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day17;

class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 17: Chronospatial Computer"; }
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

    private long registerA;
    private long registerB;
    private long registerC;

    private byte[] program = [];
    private int pointer;

    private void InitializeProgram(string input)
    {
        var parts = input.Split("\n\n");

        var registerLines = parts[0].Split('\n');
        registerA = int.Parse(registerLines[0].Split(": ")[1]);
        registerB = int.Parse(registerLines[1].Split(": ")[1]);
        registerC = int.Parse(registerLines[2].Split(": ")[1]);

        program = [.. parts[1].Split(": ")[1].Split(',').Select(byte.Parse)];

        pointer = 0;
    }

    /// <summary>
    /// Interpret an operand as a combo operand type.
    /// </summary>
    private long ComboOperand(byte operand)
    {
        if (operand >= 0 && operand <= 3)
            return operand;

        if (operand == 4)
            return registerA;
        if (operand == 5)
            return registerB;
        if (operand == 6)
            return registerC;

        throw new ArgumentException(
            $"Could not handle operand {operand}.", nameof(operand)
        );
    }

    /// <summary>
    /// Calculate register A divided by 2 power (combo) operand.
    /// </summary>
    private long DivA(byte operand)
    {
        return registerA / (long)Math.Pow(2, ComboOperand(operand));
    }

    private static long PositiveMod(long value, long modulo)
    {
        return (value % modulo + modulo) % modulo;
    }

    /// <summary>
    /// Calculate (combo) operand modulo 8, keeping lowest 3 bits.
    /// </summary>
    private long ModuloOperand(byte operand)
    {
        return PositiveMod(ComboOperand(operand), 8);
    }

    private List<byte> RunProgram()
    {
        List<byte> output = [];

        while (pointer < program.Length)
        {
            byte opcode = program[pointer], operand = program[pointer + 1];

            switch (opcode)
            {
                case 0:
                    // adv
                    registerA = DivA(operand);
                    break;
                case 1:
                    // bxl
                    registerB ^= operand;
                    break;
                case 2:
                    // bst
                    registerB = ModuloOperand(operand);
                    break;
                case 3:
                    // jnz
                    if (registerA != 0)
                    {
                        pointer = operand;
                        // skip the normal increment of pointer
                        continue;
                    }
                    break;
                case 4:
                    // bxc
                    registerB ^= registerC;
                    break;
                case 5:
                    // out
                    output.Add((byte)ModuloOperand(operand));
                    break;
                case 6:
                    // bdv
                    registerB = DivA(operand);
                    break;
                case 7:
                    // cdv
                    registerC = DivA(operand);
                    break;
                default:
                    throw new Exception($"Could not handle opcode {opcode}.");
            }

            pointer += 2;
        }

        return output;
    }

    public object PartOne(string input)
    {
        InitializeProgram(input);

        var output = RunProgram();

        // NB I had read the following to mean the commas I abstracted out
        //  were to be used to join the digits together into a string, thus
        //  join my list with empty string, but no it meant format as CSV:
        /*
        What do you get if you use commas to join the values it output into a single string?
        */
        // I'm fried
        return string.Join(',', output);
    }

    public object PartTwo(string input)
    {
        return "TODO implement";
    }
}
