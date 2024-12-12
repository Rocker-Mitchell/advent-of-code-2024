using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day12;

/// <summary>
/// Solution code for Day 12: Garden Groups
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 12: Garden Groups"; }
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

    private char[,] gardenPlots = { };

    private void ParseGardenPlots(string input)
    {
        var lines = input.Split('\n');

        gardenPlots = new char[lines.Length, lines[0].Length];

        for (int row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (int col = 0; col < line.Length; col++)
            {
                gardenPlots[row, col] = line[col];
            }
        }
    }

    private readonly record struct PlotPosition(int Row, int Col)
    {
        public PlotPosition[] GetNeighbors() => [
            new(Row, Col + 1),
            new(Row, Col - 1),
            new(Row + 1, Col),
            new(Row - 1, Col)
        ];
    }

    private bool IsInBounds(PlotPosition position)
    {
        return position.Row >= 0 && position.Row < gardenPlots.GetLength(0)
            && position.Col >= 0 && position.Col < gardenPlots.GetLength(1);
    }

    private char GetPlotType(PlotPosition plot)
    {
        return gardenPlots[plot.Row, plot.Col];
    }

    private HashSet<PlotPosition> FindRegion(PlotPosition plot)
    {
        var startingPlotType = GetPlotType(plot);

        HashSet<PlotPosition> region = [plot];

        HashSet<PlotPosition> neighborsToTest = [.. plot.GetNeighbors()];

        while (neighborsToTest.Count > 0)
        {
            // from possible neighbors, determine which are untracked region
            //  candidates
            var foundRegionPlots = (
                from neighbor in neighborsToTest
                where
                    !region.Contains(neighbor)
                    && IsInBounds(neighbor)
                    && GetPlotType(neighbor) == startingPlotType
                select neighbor
            ).ToHashSet();

            region.UnionWith(foundRegionPlots);

            // clear possible neighbors then populate with subsequent neighbors
            //  of found region plots
            var nextNeighbors = (
                from regionPlot in foundRegionPlots
                from neighbor in regionPlot.GetNeighbors()
                where !region.Contains(neighbor)
                select neighbor
            ).ToHashSet();

            neighborsToTest.Clear();
            // NB the LINQ queries depend on sourcing from this collection, and
            //  there were bugs where after clearing the queries yielded
            //  empty collections to reflect the source changing. This is why
            //  each query builds to a new object, to decouple from any changes
            //  to this object.
            neighborsToTest.UnionWith(nextNeighbors);
        }

        return region;
    }

    private HashSet<PlotPosition>[] FindRegions()
    {
        List<HashSet<PlotPosition>> regions = [];

        HashSet<PlotPosition> visited = [];

        for (int row = 0; row < gardenPlots.GetLength(0); row++)
        {
            for (int col = 0; col < gardenPlots.GetLength(1); col++)
            {
                PlotPosition testPosition = new(row, col);
                if (!visited.Contains(testPosition))
                {
                    var region = FindRegion(testPosition);
                    regions.Add(region);
                    visited.UnionWith(region);
                }
            }
        }

        return [.. regions];
    }

    private static int GetRegionArea(HashSet<PlotPosition> region)
    {
        return region.Count;
    }

    private static int GetRegionPerimeter(HashSet<PlotPosition> region)
    {
        int count = 0;

        foreach (var plot in region)
        {
            foreach (var neighbor in plot.GetNeighbors())
            {
                if (!region.Contains(neighbor))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static int GetRegionFencingPrice(HashSet<PlotPosition> region)
    {
        return GetRegionArea(region) * GetRegionPerimeter(region);
    }

    private void RegionDebug(HashSet<PlotPosition> region)
    {
        var plotType = GetPlotType(region.First());
        Console.WriteLine($" region of {plotType}:");
        Console.WriteLine(
            "  area: {0}, perimeter: {1}, price: {2}",
            GetRegionArea(region),
            GetRegionPerimeter(region),
            GetRegionFencingPrice(region)
        );
        Console.Write("  plots:");
        foreach (var plot in region)
        {
            Console.Write($" ({plot.Row}, {plot.Col})");
        }
        Console.WriteLine();
    }

    public object PartOne(string input)
    {
        ParseGardenPlots(input);

        var regions = FindRegions();
        /*
        Console.WriteLine($" number of regions found: {regions.Length}");
        foreach (var region in regions)
        {
            RegionDebug(region);
        }
        //*/
        var regionPricesQuery =
            from region in regions
            select GetRegionFencingPrice(region);

        return regionPricesQuery.Sum();
    }

    private static
    (int minRow, int maxRow, int minCol, int maxCol)
    GetRegionBounds(HashSet<PlotPosition> region)
    {
        int minRow = region.First().Row, maxRow = region.First().Row,
            minCol = region.First().Col, maxCol = region.First().Col;
        foreach (var plot in region)
        {
            minRow = Math.Min(minRow, plot.Row);
            maxRow = Math.Max(maxRow, plot.Row);
            minCol = Math.Min(minCol, plot.Col);
            maxCol = Math.Max(maxCol, plot.Col);
        }

        return (minRow, maxRow, minCol, maxCol);
    }

    private static int GetRegionSides(HashSet<PlotPosition> region)
    {
        int sides = 0;

        var (minRow, maxRow, minCol, maxCol) = GetRegionBounds(region);

        // scan rows
        for (int row = minRow; row <= maxRow; row++)
        {
            bool activeTopEdge = false, activeBottomEdge = false;
            // overshoot past maxCol to have loop handle detecting side ends
            //  if last plot lands on maxCol
            for (int col = minCol; col <= maxCol + 1; col++)
            {
                PlotPosition target = new(row, col),
                    top = new(row - 1, col),
                    bottom = new(row + 1, col);

                // evaluate top edge
                if (region.Contains(target) && !region.Contains(top))
                {
                    activeTopEdge = true;
                }
                else
                {
                    if (activeTopEdge)
                    {
                        // passed the end of a side, count it
                        sides++;
                    }
                    activeTopEdge = false;
                }

                // evaluate bottom edge
                if (region.Contains(target) && !region.Contains(bottom))
                {
                    activeBottomEdge = true;
                }
                else
                {
                    if (activeBottomEdge)
                    {
                        // passed the end of a side, count it
                        sides++;
                    }
                    activeBottomEdge = false;
                }
            }
        }

        // scan columns
        for (int col = minCol; col <= maxCol; col++)
        {
            bool activeLeftEdge = false, activeRightEdge = false;
            // overshoot past maxRow to have loop handle detecting side ends
            //  if last plot lands on maxRow
            for (int row = minRow; row <= maxRow + 1; row++)
            {
                PlotPosition target = new(row, col),
                    left = new(row, col - 1),
                    right = new(row, col + 1);

                // evaluate left edge
                if (region.Contains(target) && !region.Contains(left))
                {
                    activeLeftEdge = true;
                }
                else
                {
                    if (activeLeftEdge)
                    {
                        // passed the end of a side, count it
                        sides++;
                    }
                    activeLeftEdge = false;
                }

                // evaluate right edge
                if (region.Contains(target) && !region.Contains(right))
                {
                    activeRightEdge = true;
                }
                else
                {
                    if (activeRightEdge)
                    {
                        // passed the end of a side, count it
                        sides++;
                    }
                    activeRightEdge = false;
                }
            }
        }

        return sides;
    }

    private static int GetRegionBulkFencingPrice(HashSet<PlotPosition> region)
    {
        return GetRegionArea(region) * GetRegionSides(region);
    }

    public object PartTwo(string input)
    {
        ParseGardenPlots(input);

        var regions = FindRegions();
        var regionPricesQuery =
            from region in regions
            select GetRegionBulkFencingPrice(region);

        return regionPricesQuery.Sum();
    }
}
