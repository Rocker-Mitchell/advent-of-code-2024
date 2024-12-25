using System.Collections.Immutable;
using System.Drawing;
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
        private Point _position = position;
        private readonly bool _isWide = wide;

        public Point[] GetArea()
        {
            return _isWide ? [_position, _position + new Size(1, 0)] : [_position];
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
            _position += offset;
        }

        public int Coordinate()
        {
            return _position.X + 100 * _position.Y;
        }

        public override string ToString()
        {
            return $"Entity(area: [{string.Join(", ", GetArea())}])";
        }
    }

    private Entity? _robot;

    private Entity[] _boxes = [];

    private ImmutableHashSet<Point> _walls = [];

    private string _movementSequence = string.Empty;

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
                        _robot = new(new Point(finalX, y));
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
        _boxes = [.. boxPositions.Select(p => new Entity(p, isWideWarehouse))];
        _walls = [.. wallPoints];

        _movementSequence = string.Join(string.Empty, parts[1].Split('\n'));
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

    private (bool, List<Entity>) TryToMoveEntity(Entity entity, char direction)
    {
        var movement = GetMovement(direction);
        var potentialArea = entity.GetPotentialArea(movement);

        // can't move into a wall
        if (_walls.Any(wall => potentialArea.Contains(wall)))
        {
            return (false, []);
        }

        // detect boxes in the way and try to move them recursively
        var boxesInTheWay = Array.FindAll(
            _boxes,
            delegate(Entity box)
            {
                // NB we don't want to detect the entity itself as a box in
                //  the way, or we'll get in a circular recursion
                return !ReferenceEquals(box, entity) && box.Overlaps(potentialArea);
            }
        );
        List<Entity> allBoxesMoved = [];
        foreach (var box in boxesInTheWay)
        {
            var (moved, boxesMoved) = TryToMoveEntity(box, direction);
            allBoxesMoved.AddRange(boxesMoved);
            if (!moved)
            {
                // rollback any boxes that were moved in prior recursions
                foreach (var movedBox in allBoxesMoved)
                {
                    movedBox.Move(movement * -1);
                }

                return (false, []);
            }
        }

        entity.Move(movement);
        return (true, [entity, .. allBoxesMoved]);
    }

    private void MoveRobot(char direction)
    {
        if (_robot == null)
            throw new Exception($"Robot is not set, can't apply movement '{direction}'.");

        TryToMoveEntity(_robot, direction);
    }

    private void FollowMovementSequence()
    {
        foreach (var step in _movementSequence)
        {
            MoveRobot(step);
        }
    }

    public object PartOne(string input)
    {
        ParseInput(input);

        FollowMovementSequence();

        return _boxes.Select(box => box.Coordinate()).Sum();
    }

    public object PartTwo(string input)
    {
        ParseInput(input, true);

        FollowMovementSequence();

        return _boxes.Select(box => box.Coordinate()).Sum();
    }
}
