# Day 14: Restroom Redoubt

[link](https://adventofcode.com/2024/day/14)

## Part 1

The input is positions and velocities of robots. Example:

```
p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3
```

Position is in `p=x,y`, origin in the top-left. Velocity is in `v=x,y`, in units per second.

The room the robots are in has pre-defined dimensions: 101 by 103 for the input, but for the example it is 11 by 7.

Robots do not collide with each other. Robots "screen-wrap" to negotiate the room's walls.

The solution must determine robot positions after 100 seconds, then count the number of robots in each quadrant of the room (ignoring robots on the middle axes), then calculate the product of the counts.

## Part 2

There is a rare easter egg where most of the robots arrange into a christmas tree. The solution must determine the fewest number of seconds to pass to display this tree.
