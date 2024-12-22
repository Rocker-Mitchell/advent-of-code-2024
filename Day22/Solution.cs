using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day22;

using DifferenceSequence = (int, int, int, int);

/// <summary>
/// Solution code for Day 22: Monkey Market
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 22: Monkey Market"; }
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

    static int[] ParseBuyerInitialSecretNumbers(string input)
    {
        return input.Split('\n').Select(int.Parse).ToArray();
    }

    static int Mix(int secretNumber, int value)
    {
        return secretNumber ^ value;
    }

    static int PositiveModulo(int dividend, int divisor)
    {
        return (dividend % divisor + divisor) % divisor;
    }

    static int Prune(int secretNumber)
    {
        return PositiveModulo(secretNumber, 16777216);
    }

    static int NextSecretNumber(int secretNumber)
    {
        secretNumber = Prune(Mix(secretNumber, secretNumber * 64));
        secretNumber = Prune(Mix(secretNumber, secretNumber / 32));
        secretNumber = Prune(Mix(secretNumber, secretNumber * 2048));
        return secretNumber;
    }

    static int GenerateSecretNumber(int initialSecretNumber, int cycles = 2000)
    {
        var secretNumber = initialSecretNumber;
        for (var i = 0; i < cycles; i++)
        {
            secretNumber = NextSecretNumber(secretNumber);
        }
        return secretNumber;
    }

    public object PartOne(string input)
    {
        var buyerInitialSecretNumbers = ParseBuyerInitialSecretNumbers(input);
        var secretNumbers = buyerInitialSecretNumbers
            .Select(n => (long)GenerateSecretNumber(n))
            .ToArray();
        return secretNumbers.Sum();
    }

    static int Price(int secretNumber)
    {
        // pick out the ones digit
        return PositiveModulo(secretNumber, 10);
    }

    static Dictionary<DifferenceSequence, int> GenerateDifferenceSequenceToPriceMap(
        int initialSecretNumber,
        int cycles = 2000
    )
    {
        var prices = new List<int>(cycles);
        var secretNumber = initialSecretNumber;
        for (var i = 0; i < cycles; i++)
        {
            secretNumber = NextSecretNumber(secretNumber);
            prices.Add(Price(secretNumber));
        }

        var priceDifferences = new List<int>(cycles);
        var lastPrice = Price(initialSecretNumber);
        foreach (var price in prices)
        {
            priceDifferences.Add(price - lastPrice);
            lastPrice = price;
        }

        var differenceSequenceToPrice = new Dictionary<DifferenceSequence, int>();
        for (var i = 3; i < priceDifferences.Count; i++)
        {
            var differenceSequence = (
                priceDifferences[i - 3],
                priceDifferences[i - 2],
                priceDifferences[i - 1],
                priceDifferences[i]
            );
            if (!differenceSequenceToPrice.ContainsKey(differenceSequence))
            {
                differenceSequenceToPrice[differenceSequence] = prices[i];
            }
        }

        return differenceSequenceToPrice;
    }

    public object PartTwo(string input)
    {
        var buyerInitialSecretNumbers = ParseBuyerInitialSecretNumbers(input);
        var buyersDifferenceSequenceToPrice = buyerInitialSecretNumbers
            .Select(n => GenerateDifferenceSequenceToPriceMap(n))
            .ToArray();
        int highestEarning = buyersDifferenceSequenceToPrice
            .SelectMany(dsp => dsp.Keys)
            .Distinct()
            .Select(ds =>
                buyersDifferenceSequenceToPrice
                    .Select(buyerDsp => buyerDsp.GetValueOrDefault(ds, 0))
                    .Sum()
            )
            .Max();
        return highestEarning;
    }
}
