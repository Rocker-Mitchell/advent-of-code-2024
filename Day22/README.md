# Day 22: Monkey Market

[link](https://adventofcode.com/2024/day/22)

## Part 1

On the Monkey Exchange market, there are buyers that use pseudorandom prices.

Secret numbers are generated in sequence, deriving from the previous number. The logic to generate the next secret number is known:

* multiply n by 64, then mix into n, then prune
* divide n by 32, then floor round, then mix into n, then prune
* multiply n by 2048, then mix into n, then prune

To "mix" is the bitwise XOR of values. To "prune" is to modulo by 16777216.

> It's significant that the modulo is expected to always be positive.

The input is a list of initial secret numbers for buyers. Buyers each have time to generate 2000 new numbers a day. The solution must sum the 2000th number generated per buyer.

## Part 2

The prices of the buyer offers are the ones digit of their secret numbers, in bananas.

You have a monkey to negotiate with buyers directly. Their logic on deciding when to sell is based on the change in price: a specific sequence of four consecutive changes in price, to then immediately sell.

You could determine the first instance of the highest occurring price (must have 4 preceding changes), determine the 4 price differences sequence leading up to it, and instruct the monkey to look out for those differences.

A buyer only buys once. If the difference sequence never occurs for a buyer, the monkey moves on to the next. The monkey only takes one difference sequence, to apply for all buyers.

> The sequence yielding the highest price for one buyer wouldn't guarantee getting the most bananas across all buyers.

The solution must determine the difference sequence that will sell the most bananas. Buyers still generate 2000 secret numbers, thus 2000 price changes. The most bananas earnable will be the return value.
