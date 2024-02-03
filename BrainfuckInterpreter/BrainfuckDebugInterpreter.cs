using System.Text;

namespace BrainfuckInterpreter;

public class BrainfuckDebugInterpreter : BrainfuckInterpreterBase {
    /// <inheritdoc/>
    public override void Interpret(string programCode, StreamReader input, StreamWriter output) {
        var programOutput = new StringBuilder();
        programCode = Strip(programCode, out var posToLnCol, out var commentsByLn);
        var foundLoopStartPositions = new List<int>();
        var currentLoopStartPosition = -1;
        var foundGoToPositions = new Dictionary<int, int>();

        var memoryCells = new List<char>() { DefaultCellContent };
        var currentCell = 0;
        var previousPosition = -1;
        var position = 0;
        while (position < programCode.Length) {
            if ((previousPosition < 0 && posToLnCol[position].Item1 != 0) || (posToLnCol[previousPosition].Item1 != posToLnCol[position].Item1)) {
                WriteComment(output, commentsByLn, memoryCells, currentCell, previousPosition < 0 ? 0 : posToLnCol[previousPosition].Item1, posToLnCol[position].Item1);
            }
            previousPosition = position;

            switch (programCode[position]) {
                case GoRightCommand:
                    currentCell++;
                    if (memoryCells.Count < (currentCell + 1)) memoryCells.Add(DefaultCellContent);
                    break;
                case GoLeftCommand:
                    currentCell--;
                    if (currentCell < 0) throw new InvalidOperationException("Invalid program. Memory cell with index less then zero is referenced.");
                    break;
                case IncrementCommand:
                    memoryCells[currentCell]++;
                    break;
                case DecrementCommand:
                    memoryCells[currentCell]--;
                    break;
                case OutputCommand:
                    programOutput.Append(memoryCells[currentCell]);
                    break;
                case InputCommand:
                    memoryCells[currentCell] = (char)input.Read();
                    break;
                case LoopStartCommand:
                    WriteDebug(output, programCode, position, posToLnCol, memoryCells, currentCell);
                    if (memoryCells[currentCell] == 0) {
                        if (foundGoToPositions.TryGetValue(position, out var newPosition)) {
                            position = newPosition;
                        }
                        else {
                            var oldPosition = position;
                            position = FindEndOfLoopPosition(programCode, position);
                            foundGoToPositions.Add(oldPosition, position);
                        }
                        previousPosition = position;
                    }
                    else {
                        currentLoopStartPosition++;
                        if (foundLoopStartPositions.Count <= currentLoopStartPosition) {
                            foundLoopStartPositions.Add(position);
                        }
                        else {
                            foundLoopStartPositions[currentLoopStartPosition] = position;
                        }
                    }
                    position++;
                    continue;
                case LoopEndCommand:
                    WriteDebug(output, programCode, position, posToLnCol, memoryCells, currentCell);
                    if (memoryCells[currentCell] != 0) {
                        if (foundGoToPositions.TryGetValue(position, out var newPosition)) {
                            position = newPosition;
                        }
                        else {
                            var oldPosition = position;
                            position = foundLoopStartPositions[currentLoopStartPosition--];
                            foundGoToPositions.Add(oldPosition, position);
                        }
                        previousPosition = position;
                    }
                    position++;
                    continue;
                default:
                    throw new InvalidOperationException($"Command code[{position}] = \"{programCode[position]}\" is not defined.");
            }
            WriteDebug(output, programCode, position, posToLnCol, memoryCells, currentCell);
            position++;
        }
        WriteComment(output, commentsByLn, memoryCells, currentCell, posToLnCol[previousPosition].Item1, posToLnCol.Values.Max(x => x.Item2));
        output.WriteLine();
        output.WriteLine("Program output:");
        output.WriteLine(programOutput.ToString());
    }

    private static char ConvertToPrintable(char c) => c < 32 || c == 255 ? '' : c;
    
    private static int FindEndOfLoopPosition(string code, int position) {
        var matchingBracket = LoopEndCommand;
        var opposingBracket = LoopStartCommand;
        int counter = 1;
        do {
            position++;
            if (position < 0) throw new InvalidOperationException("Program is invalid. Loop is closed but no starting point.");
            if (code[position] == opposingBracket) { counter++; continue; }
            if (code[position] == matchingBracket) counter--;
        } while (!(counter == 0 && code[position] == matchingBracket));
        return position;
    }
    
    private static void WriteDebug(StreamWriter output, string programCode, int position, Dictionary<int, Tuple<int, int>> posToLnCol, IList<char> memoryCells, int currentCell) {
        output.WriteLine($"code[{posToLnCol[position].Item1, 3}, {posToLnCol[position].Item2, 3}]: {programCode[position]} | cell[{currentCell,3}] = {(int)memoryCells[currentCell],5} ({ConvertToPrintable(memoryCells[currentCell])})");
    }

    private static void WriteComment(StreamWriter output, Dictionary<int, string> commentsByLn, IList<char> memoryCells, int currentCell, int fromLn, int toLn) {
        for (var i = fromLn; i < toLn; i++) {
            if (commentsByLn.TryGetValue(i, out var comment)) {
                output.WriteLine($"{comment}");
                output.WriteLine($"|{string.Join("|", Enumerable.Range(0, memoryCells.Count).Select(x => x == currentCell ? @"\\/\/\/" : "       "))}|");
                output.WriteLine($"| {string.Join(" | ", memoryCells.Select(x => $"{(int)x,5}"))} |");
                output.WriteLine($"|   {string.Join("   |   ", memoryCells.Select(ConvertToPrintable))}   |");
            }
        }
    }
}
