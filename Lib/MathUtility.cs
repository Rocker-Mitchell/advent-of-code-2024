namespace AdventOfCode2024.Lib;

public static class MathUtility
{
    /// <summary>
    /// Perform a "true" modulo operation.
    /// <br/><br/>
    /// '%' in C languages will do a remainder operation, which behaves differently.
    /// For example, -1 % 5 = -1, but this function will return 4.
    /// </summary>
    public static int Mod(int dividend, int modulus)
    {
        return (dividend % modulus + modulus) % modulus;
    }

    /// <summary>
    /// Perform a "true" modulo operation.
    /// <br/><br/>
    /// '%' in C languages will do a remainder operation, which behaves differently.
    /// For example, -1 % 5 = -1, but this function will return 4.
    /// </summary>
    public static long Mod(long dividend, long modulus)
    {
        return (dividend % modulus + modulus) % modulus;
    }
}
