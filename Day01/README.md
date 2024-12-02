# Day 1: Historian Hysteria

[link](https://adventofcode.com/2024/day/1)

## Part 1

So we have a pair of lists of location IDs, side by side, input like:

```
3   4
4   3
2   5
1   3
3   9
3   3
```

To evaluate difference, pair numbers in ascending order and measure absolute difference:

```
abs(1-3) = 2
abs(2-3) = 1
abs(3-3) = 0
abs(3-4) = 1
abs(3-5) = 2
abs(4-9) = 5
```

Then get the sum of differences: in this example, 11.

## Part 2

Many IDs are similar between the lists, and now we suspect some numbers are misinterpreted handwriting.

Now to evaluate difference, add the numbers in the left multiplied by the frequency that number has in the right. For the earlier example:

```
3 * 3 = 9
4 * 1 = 4
2 * 0 = 0
1 * 0 = 0
3 * 3 = 9
3 * 3 = 9
sum = 31
```
