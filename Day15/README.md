# Day 15: Warehouse Woes

[link](https://adventofcode.com/2024/day/15)

## Part 1

The input is a map of a warehouse and a list of movements a robot in the warehouse will attempt to make, separated by an empty line.

The robot starts at `@`. If it moves into a box marked `O`, it pushes the box. A chain of boxes can be pushed. Walls marked `#` block the movement of boxes and the robot.

The list of movements is a sequence of `^`, `v`, `<`, and `>` for the direction on the map. Newlines in the sequence should be ignored.

Example:

```
########
#..O.O.#
##@.O..#
#...O..#
#.#.O..#
#...O..#
#......#
########

<^^>>>vv<v>>v<<
```

A coordinate of a box is calculated as 100 times the distance from the top edge added with the distance from the right edge: `x + 100*y`. The solution must find the sum of box coordinates when the robot stops.

## Part 2

A variation of the same input warehouse exists where everything except the robot is twice as wide: walls, boxes, and empty space. The robot starts on the left side of the widened space it's on.

With wide boxes, it's possible to contact multiple boxes to push, or one side contacting a wall to block movement.

Box coordinates for wide boxes will use closest box edges to map edges, so the leftmost space applies.

Solve sum of box coordinates for this variation of warehouse.
