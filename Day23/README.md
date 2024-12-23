# Day 23: LAN Party

[link](https://adventofcode.com/2024/day/23)

## Part 1

The input is a map of the local network. It is a list of every connection between two computers, identified with two characters, hyphen separated. Connections are not directional.

You want to locate a LAN party, so you can find groups of connected computers. Start with sets of 3 computers where each is connected to the others.

The solution should focus on sets including a computer that starts with `t`, and return the number of sets.

## Part 2

With so many sets found, it doesn't seem a reasonable way to locate the LAN party. Instead, solve for the largest set of computers connected to each other --- each computer connecting to all other computers --- then format the computer names alphabetically w/ comma separation (like `co,de,ka,ta`).

> This set is called a "clique", so this would be the maximum clique.
