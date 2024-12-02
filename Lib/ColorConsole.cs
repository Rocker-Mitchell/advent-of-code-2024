namespace AdventOfCode2024.Lib;

/// <summary>
/// Wrapper functions to add color when writing to console.
/// </summary>
static class ColorConsole
{
    public static void Write(string text = "", ConsoleColor color = ConsoleColor.Gray)
    {
        var temp = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = temp;
    }

    public static void WriteLine(string text = "", ConsoleColor color = ConsoleColor.Gray)
    {
        Write(text + '\n', color);
    }
}
