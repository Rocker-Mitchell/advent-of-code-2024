# Day 18: RAM Run

[link](https://adventofcode.com/2024/day/18)

## Part 1

The input is a line separated list of `x,y` coordinates, representing the order bytes will "fall" at an interval into a memory space. It is established the memory space is 70 by 70.

You start at the top-left corner (0, 0) and want to travel to the bottom-right corner (70, 70). As bytes fall, they make the coordinate corrupted, which you cannot enter. You can't exit the bounds of the memory space. You can't move diagonally, only left/right/up/down.

Simulate the first 1024 byte falls, then calculate the minimum number of steps to reach the bottom-right corner.

## Part 2

It's expected that eventually the byte falls will make it impossible to reach the bottom-right corner. Determine the byte coordinate of the first byte fall that will block access to this corner, formatting with the comma like `20,24`.
