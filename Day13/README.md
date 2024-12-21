# Day 13: Claw Contraption

[link](https://adventofcode.com/2024/day/13)

## Part 1

The input is data describing multiple claw machines, separated by empty lines. An example format:

```
Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400
```

The machines use two buttons --- A and B --- to move the claw right and forward by specified amounts. The machines charge 3 tokens to press A, 1 token to press B. Each machine holds one prize, and the claw must move exactly to its position to win.

For any needed convenience, each button should need to be pressed no more than 100 times to win.

Solve for the fewest tokens to spend to win all possible prizes.

## Part 2

There was a unit conversion error in the input, all prizes are 10000000000000 higher on both axes. That would revise the machine in the earlier example to have the prize at `(10000000008400, 10000000005400)`. This can change what machines are solvable now, but can also require more than 100 presses to a button.
