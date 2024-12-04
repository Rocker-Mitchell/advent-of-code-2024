using System.Linq;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day04;

/// <summary>
/// Solution code for Day 4: Ceres Search
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 4: Ceres Search"; }
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

    private static char[,] ParseWordSearch(string input)
    {
        var lines = input.Split('\n');
        var colCount = lines[0].Length;

        var wordSearch = new char[lines.Length, colCount];
        for (int row = 0; row < lines.Length; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                wordSearch[row, col] = lines[row][col];
            }
        }

        return wordSearch;
    }

    /// <summary>
    /// Count the frequency of a word in a character sequence, forward and
    /// backward.
    /// </summary>
    /// <param name="sequence">The character sequence to search.</param>
    /// <param name="word">The word to search for.</param>
    /// <returns></returns>
    private static int SearchCount(char[] sequence, string word)
    {
        // don't bother if given an empty word
        if (word.Length == 0)
        {
            return 0;
        }

        int count = 0;

        // generate reverse sequence to also search
        char[] reverse = new char[sequence.Length];
        Array.Copy(sequence, reverse, sequence.Length);
        Array.Reverse(reverse);

        foreach (var seq in new List<char[]>() { sequence, reverse })
        {
            for (int idx = 0; idx + word.Length <= seq.Length; idx++)
            {
                var check = new string(seq[idx..(idx + word.Length)]);
                if (check == word)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int SearchCountHorizontal(char[,] wordSearch, string word)
    {
        int count = 0;

        for (int row = 0; row < wordSearch.GetLength(0); row++)
        {
            var sequence = Enumerable.Range(0, wordSearch.GetLength(1))
                .Select(j => wordSearch[row, j])
                .ToArray();
            count += SearchCount(sequence, word);
        }

        return count;
    }

    private static int SearchCountVertical(char[,] wordSearch, string word)
    {
        int count = 0;

        for (int col = 0; col < wordSearch.GetLength(1); col++)
        {
            var sequence = Enumerable.Range(0, wordSearch.GetLength(0))
                .Select(i => wordSearch[i, col])
                .ToArray();
            count += SearchCount(sequence, word);
        }

        return count;
    }

    /// <summary>
    /// Helper code to extract a diagonal char array from a 2d char array/grid.
    /// <br/><br/>
    /// At default the diagonal is traversed down-right, so the coordinates
    /// should be the top-left corner. With the rotate flag, the diagonal is
    /// traversed down-left, so the coordinates should be the top-right corner.
    /// </summary>
    /// <param name="grid">The 2d array to extract from.</param>
    /// <param name="startRow">
    /// The starting row coordinate.
    /// This should be the topmost coordinate for the diagonal.
    /// </param>
    /// <param name="startCol">
    /// The starting column coordinate.
    /// This should be the leftmost coordinate for the diagonal, or the
    /// rightmost if rotate is true.
    /// </param>
    /// <param name="rotate">
    /// A flag to change from a downward-right diagonal to a downward-left
    /// diagonal.
    /// </param>
    /// <returns></returns>
    private static char[] ExtractDiagonal(char[,] grid, int startRow, int startCol, bool rotate = false)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        var diagonalChars = new List<char>();

        int row = startRow;
        int col = startCol;
        while (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            diagonalChars.Add(grid[row, col]);
            row += 1;
            col += rotate ? -1 : 1;
        }

        return [.. diagonalChars];
    }

    /// <summary>
    /// Search and count for a word along diagonals in a downward-right pattern.
    /// </summary>
    private static int SearchCountDiagonalDownRight(char[,] wordSearch, string word)
    {
        int count = 0;

        // iterate along the top row
        for (int col = 0; col < wordSearch.GetLength(1); col++)
        {
            var sequence = ExtractDiagonal(wordSearch, 0, col);
            count += SearchCount(sequence, word);
        }

        // iterate along the left column, excluding the first row
        for (int row = 1; row < wordSearch.GetLength(0); row++)
        {
            var sequence = ExtractDiagonal(wordSearch, row, 0);
            count += SearchCount(sequence, word);
        }

        return count;
    }

    /// <summary>
    /// Search and count for a word along diagonals in a downward-left pattern.
    /// </summary>
    private static int SearchCountDiagonalDownLeft(char[,] wordSearch, string word)
    {
        int count = 0;

        var rows = wordSearch.GetLength(0);
        var cols = wordSearch.GetLength(1);

        // iterate along the top row
        for (int col = 0; col < cols; col++)
        {
            var sequence = ExtractDiagonal(wordSearch, 0, col, true);
            count += SearchCount(sequence, word);
        }

        // iterate along the right column, excluding the first row
        for (int row = 1; row < rows; row++)
        {
            var sequence = ExtractDiagonal(wordSearch, row, cols - 1, true);
            count += SearchCount(sequence, word);
        }

        return count;
    }

    public object PartOne(string input)
    {
        var wordSearch = ParseWordSearch(input);

        string word = "XMAS";

        // accumulate counts from horizontal, vertical, and both diagonals
        var count = SearchCountHorizontal(wordSearch, word);
        count += SearchCountVertical(wordSearch, word);
        count += SearchCountDiagonalDownRight(wordSearch, word);
        count += SearchCountDiagonalDownLeft(wordSearch, word);

        return count;
    }

    private static bool IsCrossMas(char[,] grid)
    {
        if (grid.GetLength(0) != 3 || grid.GetLength(1) != 3)
        {
            throw new ArgumentException("The grid is expected to be of dimensions 3x3");
        }

        string word = "MAS";

        var downRight = ExtractDiagonal(grid, 0, 0);
        var downLeft = ExtractDiagonal(grid, 0, 2, true);

        return SearchCount(downRight, word) > 0 && SearchCount(downLeft, word) > 0;
    }

    public object PartTwo(string input)
    {
        var wordSearch = ParseWordSearch(input);

        int count = 0;
        for (int row = 0; row + 3 <= wordSearch.GetLength(0); row++)
        {
            for (int col = 0; col + 3 <= wordSearch.GetLength(1); col++)
            {
                var subGrid = new char[3, 3];
                foreach (var r in Enumerable.Range(0, 3))
                {
                    foreach (var c in Enumerable.Range(0, 3))
                    {
                        subGrid[r, c] = wordSearch[row + r, col + c];
                    }
                }

                if (IsCrossMas(subGrid))
                {
                    count++;
                }
            }
        }

        return count;
    }
}
