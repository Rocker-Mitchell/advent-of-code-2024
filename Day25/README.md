# Day 25: Code Chronicle

[link](https://adventofcode.com/2024/day/25)

## Part 1

The input is schematics for lock and key patterns of a 5 pin tumbler design, separated by empty lines. A schematic uses `#` as a filled space, and `.` as an empty space. Locks fill the top row while keys fill the bottom. There is no guarantee the number of locks to keys is equal (can be more of one than another). A schematic can be interpreted as sets of columns of varying heights. For locks, these columns are the pin heights. For keys, these columns are the shape of the key, heights aligning to pins.

The solution needs to find unique lock and key pairs where their heights do not overlap into each other, or worded differently the sum of heights between key and lock should not surpass the height of a schematic across all columns. Return the sum of unique pairs.
