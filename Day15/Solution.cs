using System.Collections.Immutable;
using System.Drawing;
using AdventOfCode2024.Day10;
using AdventOfCode2024.Lib;

namespace AdventOfCode2024.Day15;

/// <summary>
/// Solution code for Day 15: Warehouse Woes
/// </summary>
class Solution : ISolver
{
    public string PrintName
    {
        get { return "Day 15: Warehouse Woes"; }
    }

    // because structs inherit ValueType and pass-by-value, want a class
    //  wrapping a point for a mutable state and pass-by-reference
    private class Entity(Point position, bool wide = false)
    {
        private Point position = position;
        private readonly bool isWide = wide;

        public Point[] GetArea()
        {
            return isWide ? [position, position + new Size(1, 0)] : [position];
        }

        public bool Overlaps(Point[] points)
        {
            return GetArea().Any(points.Contains);
        }

        public Point[] GetPotentialArea(Size offset)
        {
            return [.. GetArea().Select(a => a + offset)];
        }

        public void Move(Size offset)
        {
            position += offset;
        }

        public int Coordinate()
        {
            return position.X + 100 * position.Y;
        }
    }

    private Entity? robot;

    private Entity[] boxes = [];

    private ImmutableHashSet<Point> walls = [];

    private string movementSequence = string.Empty;

    private void ParseInput(string input, bool isWideWarehouse = false)
    {
        var parts = input.Split("\n\n");

        var mapLines = parts[0].Split('\n');
        List<Point> boxPositions = [];
        List<Point> wallPoints = [];
        for (int y = 0; y < mapLines.Length; y++)
        {
            for (int x = 0; x < mapLines[y].Length; x++)
            {
                // to be twice as wide, the original x must double for a final
                //  point coordinate
                int finalX = isWideWarehouse ? x * 2 : x;
                switch (mapLines[y][x])
                {
                    case '@':
                        robot = new(new Point(finalX, y));
                        break;
                    case 'O':
                        boxPositions.Add(new Point(finalX, y));
                        break;
                    case '#':
                        wallPoints.Add(new Point(finalX, y));
                        if (isWideWarehouse)
                            // add an extra point for a double wide wall
                            wallPoints.Add(new Point(finalX + 1, y));
                        break;
                }
            }
        }
        boxes = [.. boxPositions.Select(p => new Entity(p, isWideWarehouse))];
        walls = [.. wallPoints];

        movementSequence = string.Join(string.Empty, parts[1].Split('\n'));
    }

    private static Size GetMovement(char direction)
    {
        return direction switch
        {
            '^' => new Size(0, -1),
            'v' => new Size(0, 1),
            '<' => new Size(-1, 0),
            '>' => new Size(1, 0),
            _ => throw new ArgumentException(
                $"Could not interpret movement from character '{direction}'.",
                nameof(direction)
            ),
        };
    }

    private Entity[] GetEntitiesThatWillMove(Entity pusher, char direction)
    {
        var pushArea = pusher.GetPotentialArea(GetMovement(direction));

        // can't move into a wall, return empty
        if (walls.Any(wall => pushArea.Contains(wall)))
        {
            return [];
        }

        // determine boxes that would be affected by the push area, then get
        //  their recursive entities that will move
        var affectedBoxes = Array.FindAll(
            boxes,
            delegate (Entity box) { return box.Overlaps(pushArea); }
        );
        var boxesThatWillMove = affectedBoxes.Select(
            box => GetEntitiesThatWillMove(box, direction)
        ).ToArray();

        // if any recursive result is empty, that flagged a wall; propagate
        if (boxesThatWillMove.Any(boxes => boxes.Length == 0))
        {
            return [];
        }

        // return the pusher with a flattening of affected boxes; length is
        //  at least 1 and not flagging a wall
        return [
            pusher,
            .. boxesThatWillMove.SelectMany(boxes => boxes)
        ];
    }

    private void MoveRobot(char direction)
    {
        if (robot == null)
            throw new Exception(
                $"Robot is not set, can't apply movement '{direction}'."
            );

        var entitiesToPush = GetEntitiesThatWillMove(robot, direction);
        // empty array means wall prevents movement, otherwise valid to move
        foreach (var ent in entitiesToPush)
        {
            ent.Move(GetMovement(direction));
        }
    }

    private void FollowMovementSequence()
    {
        foreach (var step in movementSequence)
        {
            try
            {
                MoveRobot(step);
            }
            catch (StackOverflowException)
            {
                DebugRoom();
            }
        }
    }

    private void DebugRoom()
    {
        foreach (int y in Enumerable.Range(0, 50))
        {
            foreach (int x in Enumerable.Range(0, 50 * 2))
            {
                Point testPoint = new(x, y);
                if (walls.Contains(testPoint))
                    Console.Write('#');
                else if (boxes.Any(box => box.Overlaps([testPoint])))
                    Console.Write('O');
                else if (robot.Overlaps([testPoint]))
                    Console.Write('@');
                else
                    Console.Write('.');
            }
            Console.WriteLine();
        }
    }

    public object PartOne(string input)
    {
        ParseInput(input);

        FollowMovementSequence();

        return boxes.Select(box => box.Coordinate()).Sum();
    }

    public object PartTwo(string input)
    {
        /*
        ParseInput(input, true);

        FollowMovementSequence();

        return boxes.Select(box => box.Coordinate()).Sum();\
        */
        // BUG StackOverflowException, having hard time debugging
        return null;
    }
}
