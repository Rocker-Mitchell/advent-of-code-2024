using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day05;

/// <summary>
/// Solution code for Day 5: Print Queue
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 5: Print Queue"; }
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

    private static (Dictionary<int, HashSet<int>>, int[][]) ParseInput(string input)
    {
        var parts = input.Split("\n\n");
        string orderInput = parts[0], updateInput = parts[1];

        var orderLines = orderInput.Split('\n');
        var orders = new Dictionary<int, HashSet<int>>(orderLines.Length);
        foreach (var order in orderLines)
        {
            var pair = order.Split('|');
            int front = int.Parse(pair[0]), back = int.Parse(pair[1]);

            // handle either creating the initial set or appending to an
            //  existing one
            if (orders.TryGetValue(front, out var afterPages))
            {
                afterPages.Add(back);
            }
            else
            {
                afterPages = [back];
                orders.Add(front, afterPages);
            }
        }

        var updateLines = updateInput.Split('\n');
        var updates = new int[updateLines.Length][];
        for (int idx = 0; idx < updateLines.Length; idx++)
        {
            updates[idx] = updateLines[idx].Split(',').Select(int.Parse).ToArray();
        }

        return (orders, updates);
    }

    private static bool IsValidUpdate(int[] update, Dictionary<int, HashSet<int>> orders)
    {
        for (int idx = 0; idx < update.Length; idx++)
        {
            var page = update[idx];
            if (orders.TryGetValue(page, out var afterPages))
            {
                foreach (var afterPage in afterPages)
                {
                    // NB relying on there not being multiple instances of a page
                    var afterIdx = Array.IndexOf(update, afterPage);

                    // if an after page is found, require its index to come
                    //  after the current index
                    if (afterIdx >= 0 && afterIdx <= idx)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public object PartOne(string input)
    {
        var (orders, updates) = ParseInput(input);

        int total = 0;
        foreach (var update in updates)
        {
            if (IsValidUpdate(update, orders))
            {
                int midpoint = update.Length / 2;
                total += update[midpoint];
            }
        }

        return total;
    }

    private static int[] FixUpdate(int[] update, Dictionary<int, HashSet<int>> orders)
    {
        var fixedUpdate = new List<int>(update.Length);

        // work through inserting each page one-by-one to valid positions
        foreach (var page in update)
        {
            if (orders.TryGetValue(page, out var afterPages))
            {
                // 1) map found afterPages to their possible indexes in fixed
                //    list
                // 2) filter out failures to find indexes (-1)
                // 3) add the length of the fixed list, in case no afterPages
                //    are in the fixed list
                // 4) get the minimum index found
                var firstMatchIdx = afterPages.Select(p => fixedUpdate.IndexOf(p))
                    .Where(p => p >= 0).Append(fixedUpdate.Count).Min();

                // insert just before the earliest found afterPage (or the
                //  end), assuming anything before can validly have a rule
                //  that this page should come after
                fixedUpdate.Insert(firstMatchIdx, page);
            }
            else
            {
                // not expecting this number to need to come before any others,
                //  add at end to guarantee it comes after any it may need to
                fixedUpdate.Add(page);
            }
        }

        return [.. fixedUpdate];
    }

    public object PartTwo(string input)
    {
        var (orders, updates) = ParseInput(input);

        int total = 0;
        foreach (var update in updates)
        {
            if (!IsValidUpdate(update, orders))
            {
                var fixedUpdate = FixUpdate(update, orders);
                int midpoint = fixedUpdate.Length / 2;
                total += fixedUpdate[midpoint];
            }
        }

        return total;
    }
}
