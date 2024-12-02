# Day 2: Red-Nosed Reports

[link](https://adventofcode.com/2024/day/2)

## Part 1

The input is multi-line reports of levels space separated like:

```
7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9
```

Reports are considered safe if:

* Levels are all increasing or decreasing across
* Adjacent levels' difference is between 1 and 3 (inclusive)

In the example:

* The 2nd report is unsafe because `2 7` is a difference of 5.
* The 3rd because `6 2` is diff 4.
* The 4th increases & decreases (`1 3` then `3 2`).
* The 5th because `4 4` is diff 0.

This means 2 reports are safe.

The solution must find the number of safe reports.

## Part 2

A tolerance is introduced into the conditions of safety. If removing a single level from the report changes it to safe, it will be counted as safe.

In the example:

* The 4th report removing the `3` would be safe.
* The 5th removing the first `4` would be safe.

The new count of safe reports is 4.
