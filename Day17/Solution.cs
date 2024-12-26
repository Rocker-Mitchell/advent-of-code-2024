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

    private long _registerA;
    private long _registerB;
    private long _registerC;

    private byte[] _program = [];

    private void InitializeProgram(string input)
    {
        var parts = input.Split("\n\n");

        var registerLines = parts[0].Split('\n');
        _registerA = int.Parse(registerLines[0].Split(": ")[1]);
        _registerB = int.Parse(registerLines[1].Split(": ")[1]);
        _registerC = int.Parse(registerLines[2].Split(": ")[1]);

        _program = [.. parts[1].Split(": ")[1].Split(',').Select(byte.Parse)];
    }

    /// <summary>
    /// Interpret an operand as a combo operand type.
    /// </summary>
    private long ComboOperand(byte operand)
    {
        if (operand < 4)
            return operand;

        if (operand == 4)
            return _registerA;
        if (operand == 5)
            return _registerB;
        if (operand == 6)
            return _registerC;

        throw new ArgumentException($"Could not handle operand {operand}.", nameof(operand));
    }

    /// <summary>
    /// Calculate register A divided by 2 power (combo) operand.
    /// </summary>
    private long DivA(byte operand)
    {
        return _registerA >> (int)ComboOperand(operand);
    }

    /// <summary>
    /// Calculate (combo) operand modulo 8, keeping lowest 3 bits.
    /// </summary>
    private long ModuloOperand(byte operand)
    {
        return ComboOperand(operand) & 7;
    }

    private List<byte> RunProgram()
    {
        List<byte> output = [];
        var pointer = 0;

        while (pointer + 1 < _program.Length)
        {
            byte opcode = _program[pointer],
                operand = _program[pointer + 1];

            switch (opcode)
            {
                case 0:
                    // adv
                    _registerA = DivA(operand);
                    break;
                case 1:
                    // bxl
                    _registerB ^= operand;
                    break;
                case 2:
                    // bst
                    _registerB = ModuloOperand(operand);
                    break;
                case 3:
                    // jnz
                    if (_registerA != 0)
                    {
                        pointer = operand;
                        // skip the normal increment of pointer
                        continue;
                    }
                    break;
                case 4:
                    // bxc
                    _registerB ^= _registerC;
                    break;
                case 5:
                    // out
                    output.Add((byte)ModuloOperand(operand));
                    break;
                case 6:
                    // bdv
                    _registerB = DivA(operand);
                    break;
                case 7:
                    // cdv
                    _registerC = DivA(operand);
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
        return string.Join(',', output);
    }

    private long GenerateDesiredRegisterA()
    {
        /*
        input: 2,4, 1,1, 7,5, 4,6, 1,4, 0,3, 5,5, 3,0
        interpretation:
            do {
                _registerB = _registerA % 8; // ModuloOperand(4);
                _registerB ^= 1;
                _registerC = _registerA >> _registerB; // DivA(5);
                _registerB ^= _registerC;
                _registerB ^= 4;
                _registerA = _registerA >> 3 // DivA(3);
                // only changing A here, so we're working through its bytes 3 at a time
                output.Add((byte) _registerB % 8); // ModuloOperand(5));
            }
            while (_registerA != 0)

        The last loop has to have register A bits 3 and up 0's to end loop
        (will bit shift right 3 to be all 0's), the unknown is the 3 least
        significant bits before the loop starts.
            - Can test values 0 through 7 to get last output matching.
        Once the 3 bits have a working value, we can shift left 3 to postulate
        the previous loop; the least significant 3 bits are the unknown again.
            - Repeat solving the 3 bits to get the desired output of that loop
                step and shifting left 3 for the next loop.
        */
        HashSet<long>[] workingRegistersByStep = new HashSet<long>[_program.Length];

        for (int step = _program.Length - 1; step >= 0; step--)
        {
            HashSet<long> priorWorkingRegisters =
                step + 1 < workingRegistersByStep.Length ? workingRegistersByStep[step + 1] : [0];
            HashSet<long> workingRegisters = [];

            foreach (var priorWorkingRegister in priorWorkingRegisters)
            {
                // evaluate with all the 3 bit values, tracking when they
                //  correctly compute the target output
                for (int value = 0; value < 8; value++)
                {
                    var currentStepRegister = (priorWorkingRegister << 3) + value;
                    _registerA = currentStepRegister;
                    _registerB = 0;
                    _registerC = 0;

                    var output = RunProgram();
                    if (output[0] == _program[step])
                    {
                        workingRegisters.Add(currentStepRegister);
                    }
                }
            }

            workingRegistersByStep[step] = workingRegisters;
        }

        return workingRegistersByStep[0].Min();
    }

    public object PartTwo(string input)
    {
        InitializeProgram(input);

        var desiredRegisterA = GenerateDesiredRegisterA();

        _registerA = desiredRegisterA;
        _registerB = 0;
        _registerC = 0;
        var output = RunProgram();
        if (output.SequenceEqual(_program))
        {
            return desiredRegisterA;
        }
        else
        {
            throw new Exception(
                $"Didn't find correct desired A register, output: [{string.Join(", ", output)}]."
            );
        }
    }
}
