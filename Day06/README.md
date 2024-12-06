# Day 6: Guard Gallivant

[link](https://adventofcode.com/2024/day/6)

## Part 1

The input is a map representing a room with a guard. The guard is represented with `^` and is assumed to be facing up in the map. Obstructions in the room are represented with `#`.

The guard has a protocol of how they move:

* If something is directly in front, turn right.
* Else walk forward.

The solution must find the number of *distinct* positions the guard walks over, including the start position.

## Part 2

A single obstruction can now be added with the intent to keep the guard in a loop within the map. It can't be placed at the guard's start or over other obstructions.

The solution must determine the count of possible positions for this new obstruction.
