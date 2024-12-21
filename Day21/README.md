# Day 21: Keypad Conundrum

[link](https://adventofcode.com/2024/day/21)

## Part 1

The input is codes for a door keypad. The keypad is most like a keyboard numpad, with the bottom/fourth row being an empty gap then `0` then `A`.

A robot arm is used to input codes on the door keypad, controlled with its own directional keypad. It lays out like keyboard directional keys, then adds `A` to the right of the up key. `A` is to activate the arm, pressing a door key. The arm starts over the door's `A` key.

A second robot arm is used to control the first, with the same keypad layout, and starts over the `A` key. And a third robot for the second.

There are usually multiple shortest possible directional keypad sequences as input. Example: door code `029A` can be input through the first robot arm with `<A^A>^^AvvvA`, `<A^A^>^AvvvA`, or `<A^A^^>AvvvA`. The robot arms are not allowed to travel over a space without a key, which can disqualify some possible sequences.

The complexity of a door code is the product of the length of the shortest sequence (for the third robot) by the numeric part of the code (for `029A` this would be 29).

The solution must determine the shortest sequences for codes, then sum the complexities as a result.

## Part 2

There are now 26 robot arms. Solve for the sum of complexities again.
