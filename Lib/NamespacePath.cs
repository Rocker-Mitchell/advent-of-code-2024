namespace AdventOfCode2024.Lib;

/// <summary>
/// Functions to interpret file paths from namespace data.
/// </summary>
static class NamespacePath
{
    /// <summary>
    /// Get a relative path for the parent folder holding a file with the type
    /// declaration.
    /// <br/><br/>
    /// It is expected that the type declaration is defined in a namespace
    /// that satisfies styling rules for the file's folder structure: the
    /// project name followed by the folder names, dot separated, case
    /// sensitive.
    /// </summary>
    /// <param name="t">A type declaration to evaluate.</param>
    /// <returns>The relative path for the parent folder.</returns>
    public static string GetFolderPathFromType(Type t)
    {
        string fullName = t.Namespace ?? "";

        // split on periods, and skip the project name in the first index
        return Path.Combine(fullName.Split('.')[1..]);
    }
}
