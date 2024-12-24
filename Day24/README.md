# Day 24: Crossed Wires

[link](https://adventofcode.com/2024/day/24)

## Part 1

The input is a system of initial wire values and logic gate connections. Wires are named alphanumerically like `y02` or `frj`, and connect to logic gates.

Gate types available are AND, OR, and XOR. Gates wait for both inputs to be set before outputting, and will persist output until the whole system is reset. Wires carry 0, 1, or no value, and each connect to at most one gate output but can connect many gate inputs.

Ultimately, the system tries to produce a binary number with output wires formatted as `z` with two digits. `z00` is the least significant bit, continuing to the most significant in ascending wire name.

The solution must simulate the system and return the decimal format of the binary number output.

## Part 2

Closer inspection determines the system is trying to add two binary numbers.
One input number is on wires starting with `x`, and the other starting with `y`. These both have `00` as the least significant bit.

The initial wire values in the input are just one instance of a wrong addition operation.

Forensic analysis shows exactly 4 pairs of gates had their output wires swapped (pairs holding gates unique from each other).

The solution must determine the pairs of gates to swap outputs to correct the system, then format the outputs (8 total) in a comma separated list in alphanumeric order.

> We could assume the intended system should operate and be assembled like a Ripple Carry Adder.
