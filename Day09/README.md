# Day 9: Disk Fragmenter

[link](https://adventofcode.com/2024/day/9)

## Part 1

The input is a single-line disk map presented as a string of digits. Digits are block lengths of files then free space alternating. The files are implicitly ID'd by zero-based index for their order in the map.

The file blocks should be moved from the end of the disk to the leftmost free space block, until there are no more free space gaps. Example of steps with map `12345`:

```
0..111....22222
02.111....2222.
022111....222..
0221112...22...
02211122..2....
022111222......
```

After block movement, a checksum is calculated by summing the products between a block position (zero-based) & its ID, skipping free spaces. The example map `12345` would have a checksum of 1928.

## Part 2

The prior block moving logic introduced file system fragmentation.

Revise the block moving logic to preserve whole files of blocks instead. A file only moves to a free space if the file size is equal to or smaller than the free space available. The logic works over decreasing file IDs once, and only moves a file once. An example of steps with map `2333133121414131402`:

```
00...111...2...333.44.5555.6666.777.888899
0099.111...2...333.44.5555.6666.777.8888..
0099.1117772...333.44.5555.6666.....8888..
0099.111777244.333....5555.6666.....8888..
00992111777.44.333....5555.6666.....8888..
```
