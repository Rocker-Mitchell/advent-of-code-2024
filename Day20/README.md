# Day 20: Race Condition

[link](https://adventofcode.com/2024/day/20)

## Part 1

The input is a map of a racetrack. It is made of track `.`, a start point `S`, an end point `E`, and walls `#`. There should be only one path through from start to end.

Movement is in directions up/down/left/right, costing 1 step.

A rule is added to allow cheating. Exactly once during a race, collision with walls is disabled for 2 steps, and must finish on track. This effectively means walking through a wall one point thick. Cheats can be uniquely represented by a start position & end position.

> The examples numbering the cheat path confused. The start considered for uniqueness isn't the wall passed through, it's the track point that is traversed from into the wall.

The solution must determine the number of cheats that save at least 100 steps.

## Part 2

The cheat rule has changed: the cheat can now last up to 20 steps. Cheats with different paths are considered equivalent if their start and end match.

> Relevant question: does the cheat path have to be only over walls? The page example, when illustrating an alternate path with equivalent start & end, does have the second cheat step over a track point.

Solve for the number of cheats saving at least 100 steps with this revised rule.
