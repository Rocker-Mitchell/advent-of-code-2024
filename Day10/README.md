# Day 10: Hoof It

[link](https://adventofcode.com/2024/day/10)

## Part 1

The input is a topographic map using digits to represent height at positions, 0 the lowest and 9 the highest.

Trail paths should be determined in the map that satisfy:

* start at 0, end at 9 height
* increases 1 height per step
* steps don't move diagonally

Trailheads are start positions to one or more trail paths. Trailheads have scores based on how many 9 height positions can be reached by its paths.

The solution must find the sum of all trailhead scores in the map.

## Part 2

Trailheads can now also be measured by its rating, or the number of distinct trail paths. Notable is these trails can share the same endpoint.

Now solve for the sum of all trailhead ratings.
