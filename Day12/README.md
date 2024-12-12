# Day 12: Garden Groups

[link](https://adventofcode.com/2024/day/12)

## Part 1

The input is a map of garden plots. The type of plant in a plot is noted by a letter.

Multiple plots with the same plant touching along edges (horizontal or vertical) form a region. Separated regions can have the same plant, and regions can appear within others like islands.

A region will have an area and perimeter. The area is the number of plots. With plots being square, the perimeter is the number of plot sides not touching a plot in its region.

Fencing prices for regions are calculated by the area times perimeter. The solution must find the total price to fence all regions.

## Part 2

The amount of fencing is high enough to qualify for a bulk discount.

The pricing calculation changes from using the perimeter to the number of sides of the region. A side is a continuous straight boundary, that can be shared with many plots. For example, a perfectly rectangular region would have 4 sides.

Now solve for the total price with the bulk discount.
