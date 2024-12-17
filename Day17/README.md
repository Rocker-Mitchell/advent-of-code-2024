# Day 17: Chronospatial Computer

[link](https://adventofcode.com/2024/day/17)

## Part 1

The input is information about a program trying to run. Example:

```
Register A: 729
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0
```

The three registers hold integers, and the program is a list of instructions in 3-bit number representations.

An instruction takes a number as its opcode, and the following number as its operand. The instruction pointer starts at 0 to use the first number as the opcode, and normally increments by 2 for the next instruction. Attempting to read an opcode past the end of the program will halt.

There are two types of operand, depending on the instruction. A literal operand is the value itself. A combo operand maps:

* 0 through 3 as literal values
* 4, 5, 6 as values in registers A, B, C respectively
* 7 as reserved, should not appear in valid programs

The instructions are:

* 0=adv: division of A by 2^comboOperand, saved w/ truncation to A
* 1=bxl: bitwise XOR of B and literalOperand, saved to B
* 2=bst: modulo comboOperand by 8 (keeping lowest 3 bits), save to B
* 3=jnz: evaluates A; if 0 do nothing, else jump to literalOperand overriding normal increment of 2
* 4=bxc: bitwise XOR of B and C, save to B
* 5=out: modulo comboOperand by 8, and output result; multiple outputs are comma separated
* 6=bdv: division of A by 2^comboOperand, saved w/ truncation to B
* 7=cdv: division of A by 2^comboOperand, saved w/ truncation to C

The solution must find the program's output, then ~~concat the output digits together~~ return the numbers joined with commas, like `0,4,9,3,0`.

## Part 2

This program is supposed to generate output that matches the instructions. Only changing register A, find the lowest positive initial value to output a copy of the instructions.
