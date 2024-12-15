var mapInput = File.ReadAllLines(Path.Join(Directory.GetCurrentDirectory(), "input1.txt")); // Read input data
using var commandsReader = new StreamReader(Path.Join(Directory.GetCurrentDirectory(), "input2.txt")); // Input is split in two files, one is just a map other contains all commands

var directions = new[] { (-1, 0), (0, 1), (1, 0), (0, -1) }; // Directions array for use in methods to check if move is possible and act accordingly

// Data preparation
var mapPart1 = mapInput.Select(s => s.ToCharArray()).ToArray();
var mapPart2 = new char[mapPart1.Length][];
var robotPosPart1 = new int[] {0, 0};
var robotPosPart2 = new int[] {0, 0};
for (var i = 0; i < mapPart1.Length; i++)
{
    mapPart2[i] = new char[mapPart1[i].Length * 2] ;
    for (var j = 0; j < mapPart1[i].Length; j++)
    {
        if (mapPart1[i][j] == '@')
        {
            robotPosPart1 = [i, j];
            robotPosPart2 = [i, j * 2];
        }

        switch (mapPart1[i][j])
        {
            case '#':
            {
                mapPart2[i][j * 2] = '#';
                mapPart2[i][j * 2 + 1] = '#';
                break;
            }
            case 'O':
            {
                mapPart2[i][j * 2] = '[';
                mapPart2[i][j * 2 + 1] = ']';
                break;
            }
            case '.':
            {
                mapPart2[i][j * 2] = '.';
                mapPart2[i][j * 2 + 1] = '.';
                break;
            }
            case '@':
            {
                mapPart2[i][j * 2] = '@';
                mapPart2[i][j * 2 + 1] = '.';
                break;
            }
        }
        
    }
}


// Main loop
while (!commandsReader.EndOfStream)
{
    var direction = commandsReader.Read();
    switch (direction)
    {
        case '^':
        {
            var resultPart1 = TryMovePart1(robotPosPart1, Direction.Up);
            var resultPart2 = TryMovePart2(robotPosPart2, Direction.Up);
            if (resultPart1)
            {
                robotPosPart1[0]--;
            }

            if (resultPart2)
            {
                robotPosPart2[0]--;
            }
            break;
        }
        case '>':
        {
            var resultPart1 = TryMovePart1(robotPosPart1, Direction.Right);
            var resultPart2 = TryMovePart2(robotPosPart2, Direction.Right);
            if (resultPart1)
            {
                robotPosPart1[1]++;
            }

            if (resultPart2)
            {
                robotPosPart2[1]++;
            }

            break;
        }
        case '<':
        {
            var resultPart1 = TryMovePart1(robotPosPart1, Direction.Left);
            var resultPart2 = TryMovePart2(robotPosPart2, Direction.Left);
            if (resultPart1)
            {
                robotPosPart1[1]--;
            }

            if (resultPart2)
            {
                robotPosPart2[1]--;
            }

            break;
        }
        case 'v':
        {
            var resultPart1 = TryMovePart1(robotPosPart1, Direction.Down);
            var resultPart2 = TryMovePart2(robotPosPart2, Direction.Down);
            if (resultPart1)
            {
                robotPosPart1[0]++;
            }

            if (resultPart2)
            {
                robotPosPart2[0]++;
            }
            break;
        }
    }
}

// Calculate and print answers
long part1Answer = 0, part2Answer = 0;
for (var i = 0; i < mapPart1.Length; i++)
{
    for (var j = 0; j < mapPart1[i].Length; j++)
    {
        if (mapPart1[i][j] == 'O')
        {
            part1Answer += 100 * i + j;
        }
    }
}

for (var i = 0; i < mapPart2.Length; i++)
{
    for (var j = 0; j < mapPart2[i].Length; j++)
    {
        if (mapPart2[i][j] == '[')
        {
            part2Answer += 100 * i + j;
        }
    }
}
Console.WriteLine($"First part: {part1Answer}");
Console.WriteLine($"Second part: {part2Answer}");
return;


// Method to move robot and all boxes in his way if it's possible in part 1
bool TryMovePart1(int[] objectPos, Direction direction)
{
    var movedX = objectPos[0] + directions[(int)direction].Item1;
    var movedY = objectPos[1] + directions[(int)direction].Item2;
    switch (mapPart1[movedX][movedY])
    {
        case 'O':
        {
            var result = TryMovePart1([movedX, movedY], direction);
            if(!result) return false;
            break;
        }
        case '#':
            return false;
    }
    (mapPart1[movedX][movedY], mapPart1[objectPos[0]][objectPos[1]]) = (mapPart1[objectPos[0]][objectPos[1]], mapPart1[movedX][movedY]);
    return true;
}

// Method to move robot and all boxes in his way if it's possible in part 2
bool TryMovePart2(int[] objectPos, Direction direction)
{
    var movedX = objectPos[0] + directions[(int)direction].Item1;
    var movedY = objectPos[1] + directions[(int)direction].Item2;
    switch (mapPart2[movedX][movedY])
    {
        case ']':
        {
            if(direction is Direction.Up or Direction.Down)
            {
                var leftMovePossible = IsMovePossible([movedX, movedY - 1], direction);
                var rightMovePossible = IsMovePossible([movedX, movedY], direction);
                if (!(leftMovePossible || rightMovePossible)) return false;
                TryMovePart2([movedX, movedY - 1], direction);
                TryMovePart2([movedX, movedY], direction);
            }
            else
            {
                var result = TryMovePart2([movedX, movedY], direction);
                if(!result) return false;
            }

            break;
        }
        case '[':
        {
            if(direction is Direction.Up or Direction.Down)
            {
                var leftMovePossible = IsMovePossible([movedX, movedY], direction);
                var rightMovePossible = IsMovePossible([movedX, movedY + 1], direction);
                if (!(leftMovePossible || rightMovePossible)) return false;
                TryMovePart2([movedX, movedY], direction);
                TryMovePart2([movedX, movedY + 1], direction);
            }
            else
            {
                var result = TryMovePart2([movedX, movedY], direction);
                if(!result) return false;
            }

            break;
        }
        case '#':
            return false;
    }
    (mapPart2[movedX][movedY], mapPart2[objectPos[0]][objectPos[1]]) = (mapPart2[objectPos[0]][objectPos[1]], mapPart2[movedX][movedY]);
    return true;
}

// Helper function to check if all boxes in group can be moved before really moving them
bool IsMovePossible(int[] targetPos, Direction direction)
{
    return mapPart2[targetPos[0]][targetPos[1]] switch
    {
        '.' => true,
        '#' => false,
        '[' => IsMovePossible(
                   [targetPos[0] + directions[(int)direction].Item1, targetPos[1] + directions[(int)direction].Item2],
                   direction) &&
               IsMovePossible(
                   [
                       targetPos[0] + directions[(int)direction].Item1,
                       targetPos[1] + directions[(int)direction].Item2 + 1
                   ],
                   direction),
        ']' => IsMovePossible(
                   [targetPos[0] + directions[(int)direction].Item1, targetPos[1] + directions[(int)direction].Item2],
                   direction) &&
               IsMovePossible(
                   [
                       targetPos[0] + directions[(int)direction].Item1,
                       targetPos[1] + directions[(int)direction].Item2 - 1
                   ],
                   direction),
        _ => false
    };
}

// Simple enum to encode directions array index
enum Direction
{
    Up,
    Right,
    Down,
    Left,
}
