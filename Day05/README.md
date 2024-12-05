# Day 5: Print Queue

[link](https://adventofcode.com/2024/day/5)

## Part 1

The input is instructions on page order rules & pages for updates, separated by two newlines.

Page order rules are formatted as `X|Y` pairs per line, where page X must be printed at some point before page Y if both are in an update.

> Note: page numbers can occur multiple times among pairs to apply many ordering rules.

Pages for updates are formatted as CSV per line.

The solution must first identify valid page updates, then extract the middle page number of each update, and finally sum these together as the answer.

## Part 2

Now, process incorrectly ordered updates by rearranging their pages to be valid, then solve for the sum of their middle page numbers.
