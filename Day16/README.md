# Day 16: Reindeer Maze

[link](https://adventofcode.com/2024/day/16)

## Part 1

The input is a map of a maze. `#` is a wall, `S` is the start, `E` is the end. The reindeer that's in the maze starts by facing east.

The reindeer accumulates a score by the actions they make: moving forward one space is 1 point, turning 90 degrees is 1000 points.

The solution must determine the lowest score possible to finish the maze.

## Part 2

The solution must now determine all spaces that contribute to one of any of the lowest scoring paths, then return the count.
