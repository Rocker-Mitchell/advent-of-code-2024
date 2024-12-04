# Day 4: Ceres Search

[link](https://adventofcode.com/2024/day/4)

## Part 1

The input is a word search. Words can be horizontal, vertical, diagonal, backwards, and overlapping. Words can occur multiple times. It looks like all characters are capitalized.

An example where `XMAS` occurs 18 times:

```
MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX
```

Which can be adjusted to highlight the words:

```
....XXMAS.
.SAMXMS...
...S..A...
..A.A.MS.X
XMASAMX.MM
X.....XA.A
S.S.S.S.SS
.A.A.A.A.A
..M.M.M.MM
.X.X.XMASX
```

The individual counts are:

* horizontal: 3
* horizontal back: 2
* vertical: 1
* vertical back: 2
* down-right: 1
* down-right back: 4
* down-left: 1
* down-left back: 4

The solution must find how many times `XMAS` appears

## Part 2

The corrected instructions are to find two `MAS` words overlapping to make an X, called an `X-MAS`, like:

```
M.S
.A.
M.S
```

The earlier example has 9 `X-MAS`, now highlighted as:

```
.M.S......
..A..MSMS.
.M.S.MAA..
..A.ASMSM.
.M.S.M....
..........
S.S.S.S.S.
.A.A.A.A..
M.M.M.M.M.
..........
```
