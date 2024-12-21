using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Drawing;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day21;

/// <summary>
/// Solution code for Day 21: Keypad Conundrum
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 21: Keypad Conundrum"; }
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

    static string[] ParseDoorCodes(string input)
    {
        return input.Split('\n');
    }

    static readonly ReadOnlyDictionary<char, Point> doorKeypadLayout = new Dictionary<char, Point>()
    {
        { '7', new(0, 0) },
        { '8', new(1, 0) },
        { '9', new(2, 0) },
        { '4', new(0, 1) },
        { '5', new(1, 1) },
        { '6', new(2, 1) },
        { '1', new(0, 2) },
        { '2', new(1, 2) },
        { '3', new(2, 2) },
        { '0', new(1, 3) },
        { 'A', new(2, 3) },
    }.AsReadOnly();

    static readonly ReadOnlyDictionary<char, Point> directionalKeypadLayout = new Dictionary<
        char,
        Point
    >()
    {
        { '^', new(1, 0) },
        { 'A', new(2, 0) },
        { '<', new(0, 1) },
        { 'v', new(1, 1) },
        { '>', new(2, 1) },
    }.AsReadOnly();

    static readonly ConcurrentDictionary<
        (char a, int aCount, char b, int bCount),
        IEnumerable<string>
    > _charPairPermutationsCache = [];

    static IEnumerable<string> GenerateCharPairPermutations(char a, int aCount, char b, int bCount)
    {
        return _charPairPermutationsCache.GetOrAdd(
            (a, aCount, b, bCount),
            key =>
            {
                List<string> permutations = [];

                // base case
                if (aCount == 0 && bCount == 0)
                {
                    permutations.Add("");
                }

                if (aCount > 0)
                {
                    foreach (
                        var permutation in GenerateCharPairPermutations(a, aCount - 1, b, bCount)
                    )
                    {
                        permutations.Add(a + permutation);
                    }
                }

                if (bCount > 0)
                {
                    foreach (
                        var permutation in GenerateCharPairPermutations(a, aCount, b, bCount - 1)
                    )
                    {
                        permutations.Add(b + permutation);
                    }
                }

                return permutations;
            }
        );
    }

    static readonly ConcurrentDictionary<
        (ReadOnlyDictionary<char, Point> keypadLayout, char startChar, char endChar),
        IEnumerable<string>
    > _directionSequencesCache = [];

    static IEnumerable<string> GenerateDirectionSequences(
        ReadOnlyDictionary<char, Point> keypadLayout,
        char startChar,
        char endChar
    )
    {
        return _directionSequencesCache.GetOrAdd(
            (keypadLayout, startChar, endChar),
            key =>
            {
                Point start = keypadLayout[startChar],
                    end = keypadLayout[endChar];
                int horizontalLength = Math.Abs(end.X - start.X),
                    verticalLength = Math.Abs(end.Y - start.Y);
                char horizontalDirection = end.X > start.X ? '>' : '<',
                    verticalDirection = end.Y > start.Y ? 'v' : '^';

                return GenerateCharPairPermutations(
                        horizontalDirection,
                        horizontalLength,
                        verticalDirection,
                        verticalLength
                    )
                    .Where(seq =>
                    {
                        Point current = start;
                        foreach (var c in seq)
                        {
                            current += c switch
                            {
                                '>' => new Size(1, 0),
                                '<' => new Size(-1, 0),
                                'v' => new Size(0, 1),
                                '^' => new Size(0, -1),
                                _ => throw new Exception($"Couldn't parse character '{c}'."),
                            };

                            if (!keypadLayout.Values.Contains(current))
                                return false;
                        }
                        return true;
                    });
            }
        );
    }

    static readonly ConcurrentDictionary<
        (ReadOnlyDictionary<char, Point> keypadLayout, char startChar, char endChar, int depth),
        long
    > _deepSequenceCache = [];

    static long GetShortestDeepSequenceLength(
        ReadOnlyDictionary<char, Point> keypadLayout,
        char startChar,
        char endChar,
        int depth
    )
    {
        return _deepSequenceCache.GetOrAdd(
            (keypadLayout, startChar, endChar, depth),
            key =>
            {
                // base case
                if (depth == 0)
                {
                    // all direction sequences should be the same length
                    // add 1 for the appended 'A' of a full sequence
                    return GenerateDirectionSequences(keypadLayout, startChar, endChar)
                            .First()
                            .Length + 1;
                }

                var directionSequences = GenerateDirectionSequences(
                    keypadLayout,
                    startChar,
                    endChar
                );
                var sequenceLengths = directionSequences
                    .Select(seq => seq + 'A')
                    .Select(seq =>
                    {
                        long total = 0;
                        var previous = 'A';
                        foreach (var c in seq)
                        {
                            total += GetShortestDeepSequenceLength(
                                directionalKeypadLayout,
                                previous,
                                c,
                                depth - 1
                            );
                            previous = c;
                        }
                        return total;
                    });
                return sequenceLengths.Min();
            }
        );
    }

    static long GetShortestFullSequenceForDoorKey(
        char doorKey,
        char previous = 'A',
        int robotLayers = 3
    )
    {
        return GetShortestDeepSequenceLength(doorKeypadLayout, previous, doorKey, robotLayers - 1);
    }

    static long DoorCodeComplexity(string doorCode, int? robotLayers = null)
    {
        // expect to trim trailing 'A' for number parsing
        int doorValue = int.Parse(doorCode[..^1]);

        long sequenceLength = 0;
        for (int i = 0; i < doorCode.Length; i++)
        {
            char previous = i == 0 ? 'A' : doorCode[i - 1];
            sequenceLength += robotLayers.HasValue
                ? GetShortestFullSequenceForDoorKey(doorCode[i], previous, robotLayers.Value)
                : GetShortestFullSequenceForDoorKey(doorCode[i], previous);
        }

        return sequenceLength * doorValue;
    }

    public object PartOne(string input)
    {
        var doorCodes = ParseDoorCodes(input);
        var doorComplexities = doorCodes.Select(dc => DoorCodeComplexity(dc));
        return doorComplexities.Sum();
    }

    public object PartTwo(string input)
    {
        var doorCodes = ParseDoorCodes(input);
        var doorComplexities = doorCodes.Select(dc => DoorCodeComplexity(dc, 26));
        return doorComplexities.Sum();
    }
}
