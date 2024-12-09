using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day09;

readonly struct FreeSpaceWithFiles(int size)
{
    public int Size { get; init; } = size;
    public List<(int id, int size)> Files { get; init; } = [];

    public int RemainingSize() => Size - Files.Select(f => f.size).Sum();
}

/// <summary>
/// Solution code for Day 9: Disk Fragmenter
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 9: Disk Fragmenter"; }
    }

    private int[] diskMap = [];

    private void ParseDiskMap(string input)
    {
        diskMap = input.ToCharArray()
            .Select(c =>
            {
                var v = char.GetNumericValue(c);
                if (v < 0)
                    throw new ArgumentException(
                        $"Failed to parse digit from character: {c}",
                        nameof(input)
                    );
                return (int)v;
            }).ToArray();

        SetupBlockSizeLists();
    }

    private readonly List<int> fileBlockSizes = [];
    private readonly List<int> freeBlockSizes = [];

    private void SetupBlockSizeLists()
    {
        var targetCapacity = diskMap.Length / 2;
        fileBlockSizes.Clear();
        fileBlockSizes.EnsureCapacity(targetCapacity);
        freeBlockSizes.Clear();
        freeBlockSizes.EnsureCapacity(targetCapacity);

        for (int idx = 0; idx < diskMap.Length; idx++)
        {
            if (idx % 2 == 0)
            {
                fileBlockSizes.Add(diskMap[idx]);
            }
            else
            {
                freeBlockSizes.Add(diskMap[idx]);
            }
        }
    }

    private int[] GetFragmentMovedBlockIds()
    {
        // capacity will likely be longer than length, but want a good start
        List<int> blockIds = new(diskMap.Length);

        // want block size list instances I can modify w/o changing source
        List<int> copyFileBlockSizes = [.. fileBlockSizes];
        List<int> copyFreeBlockSizes = [.. freeBlockSizes];

        bool insertingFileBlocks = true;
        int firstFileId = 0;
        while (copyFileBlockSizes.Count > 0)
        {
            if (insertingFileBlocks)
            {
                // insert repeating first ID times block size, remove size
                //  from list, and increment first ID
                blockIds.AddRange(
                    Enumerable.Repeat(firstFileId, copyFileBlockSizes[0])
                );
                copyFileBlockSizes.RemoveAt(0);
                firstFileId++;

                // flip to working on free blocks
                insertingFileBlocks = false;
            }
            else
            {
                if (copyFreeBlockSizes[0] == 0)
                {
                    // no work, remove size from list & flip to working on
                    //  file blocks
                    copyFreeBlockSizes.RemoveAt(0);
                    insertingFileBlocks = true;
                }
                else if (copyFileBlockSizes[^1] == 0)
                {
                    // no blocks available from last size, remove from list
                    //  and try again in next loop
                    copyFileBlockSizes.RemoveAt(copyFileBlockSizes.Count - 1);
                }
                else
                {
                    // push last file ID, then decrement relevant file sizes
                    blockIds.Add(firstFileId + copyFileBlockSizes.Count - 1);
                    copyFileBlockSizes[^1]--;
                    copyFreeBlockSizes[0]--;
                }
            }
        }

        return [.. blockIds];
    }

    private static long GetChecksum(int[] blockIds)
    {
        return blockIds.Select((id, index) => (long)(id * index)).Sum();
    }

    public object PartOne(string input)
    {
        ParseDiskMap(input);

        var blockIds = GetFragmentMovedBlockIds();

        return GetChecksum(blockIds);
    }

    private int[] GetWholeMovedBlockIds()
    {
        FreeSpaceWithFiles[] movedFiles = freeBlockSizes.Select(
                size => new FreeSpaceWithFiles(size)
            ).ToArray();

        HashSet<int> movedFileIds = [];

        // work through each file ID descending, tracking where it can be moved;
        //  skip ID 0 since it should never move
        for (int fileId = fileBlockSizes.Count - 1; fileId > 0; fileId--)
        {
            var fileSize = fileBlockSizes[fileId];

            int usableFreeIdx = Array.FindIndex(
                movedFiles,
                freeSpace => freeSpace.RemainingSize() >= fileSize
            );

            // only move to free space before ID, not after
            if (usableFreeIdx >= 0 && usableFreeIdx < fileId)
            {
                movedFiles[usableFreeIdx].Files.Add((id: fileId, size: fileSize));
                movedFileIds.Add(fileId);
            }
        }

        List<int> blockIds = new(diskMap.Length);

        for (int fileId = 0; fileId < fileBlockSizes.Count; fileId++)
        {
            // insert repeating sequence of file ID if not moved, 0's otherwise
            blockIds.AddRange(
                Enumerable.Repeat(
                    !movedFileIds.Contains(fileId) ? fileId : 0,
                    fileBlockSizes[fileId]
                )
            );

            // expect last file ID has no counterpart free space to handle
            if (fileId < movedFiles.Length)
            {
                // insert any moved files in the free space
                foreach (var (id, size) in movedFiles[fileId].Files)
                {
                    blockIds.AddRange(Enumerable.Repeat(id, size));
                }

                // then any remaining free space fills with 0's
                blockIds.AddRange(Enumerable.Repeat(0, movedFiles[fileId].RemainingSize()));
            }
        }

        return [.. blockIds];
    }

    public object PartTwo(string input)
    {
        ParseDiskMap(input);

        var blockIds = GetWholeMovedBlockIds();

        return GetChecksum(blockIds);
    }
}
