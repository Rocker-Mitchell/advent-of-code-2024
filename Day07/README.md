# Day 7: Bridge Repair

[link](https://adventofcode.com/2024/day/7)

## Part 1

The input is test values paired with sequences of numbers. The goal is to determine the operations to apply across the sequence to equal the related test value, if possible.

* Operators all evaluate left to right, to ignore precedence rules.
* The sequence can't be reordered.
* The operators available are addition and multiplication.

> The values are large enough to not fit within 32-bit integers

The solution must find the test values possible to solve and sum them together.

## Part 2

A third operator is introduced, concatenation (`||`). This stitches together the digits of two values into one, like string concatenation.

Resolve the sum with the concatenation operator factored in.
