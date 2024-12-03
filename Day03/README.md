# Day 3: Mull It Over

[link](https://adventofcode.com/2024/day/3)

## Part 1

The input is corrupted program memory made of many multiplication instructions with invalid characters around them. The valid format looks like `mul(44,46)`, with invalid character formats that look like `mul(4*`, `mul(6,9!`, `?(12,34)`, and `mul ( 2 , 4 )`.

The solution should be the sum of all valid multiply instructions.

## Part 2

Other instructions are in the input and should now be handled:

* `do()` to enable future `mul` instructions.
* `don't()` to disable future `mul` instructions.

The beginning of the input assumes that `mul` instructions are enabled.
