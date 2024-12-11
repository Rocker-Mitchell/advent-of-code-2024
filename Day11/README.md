# Day 11: Plutonian Pebbles

[link](https://adventofcode.com/2024/day/11)

## Part 1

There are quantum-affected engraved stones that change between eye blinks, with a logic to how each stone determines its change (according to the first applicable):

* If engraved with 0, replace with 1.
* If engraving has even number of digits, split into two stones, the left half digits on the left stone, & right half digits on the right stone. Any leading zeros get removed (1000 splits to 10 & 0).
* Otherwise, replace with the old engraving times 2024.

The stones order is preserved through changes.

The input is a space separated list of numbers engraved on stones. The solution must determine how many stones there will be after 25 blinks.

## Part 2

Now solve for 75 blinks.
