using System.Text;
using System.Text.RegularExpressions;

namespace BrainfuckInterpreter;

public class BrainfuckDebugInterpreter : BrainfuckInterpreterBase {
    private static readonly Regex StopDebuggingRegex = new Regex(@"\{STOP_DEBUGGING\}");
    private static readonly Regex StopDebuggingParametrizedRegex = new Regex(@"\{STOP_DEBUGGING\(CELL\[(\d+)\]==(\d+)\)\}");
    private static readonly Regex StartDebuggingRegex = new Regex(@"\{START_DEBUGGING\}");
    private static readonly Regex StartDebuggingParametrizedRegex = new Regex(@"\{START_DEBUGGING\(CELL\[(\d+)\]==(\d+)\)\}");
    private static readonly Regex BreakRegex = new Regex(@"\{BREAK\}");
    private static readonly Regex BreakParametrizedRegex = new Regex(@"\{BREAK\(CELL\[(\d+)\]==(\d+)\)\}");

    /// <inheritdoc/>
    public override void Interpret(string programCode, StreamReader input, StreamWriter output) {
        var programOutput = new StringBuilder();
        programCode = Strip(programCode, out var posToLnCol, out var commentsByLn);
        var foundLoopStartPositions = new List<int>();
        var currentLoopStartPosition = -1;
        var foundGoToPositions = new Dictionary<int, int>();

        var isWriteDebug = true;
        var memoryCells = new List<char>() { DefaultCellContent };
        var currentCell = 0;
        var previousPosition = -1;
        var position = 0;
        while (position < programCode.Length) {
            if ((previousPosition < 0 && posToLnCol[position].Item1 != 0) || (posToLnCol[previousPosition].Item1 != posToLnCol[position].Item1)) {
                WriteComments(ref isWriteDebug, input, output, commentsByLn, memoryCells, currentCell, previousPosition < 0 ? 0 : posToLnCol[previousPosition].Item1, posToLnCol[position].Item1);
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
                    WriteDebug(isWriteDebug, output, programCode, position, posToLnCol, memoryCells, currentCell);
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
                    WriteDebug(isWriteDebug, output, programCode, position, posToLnCol, memoryCells, currentCell);
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
            WriteDebug(isWriteDebug, output, programCode, position, posToLnCol, memoryCells, currentCell);
            position++;
        }
        WriteComments(ref isWriteDebug, input, output, commentsByLn, memoryCells, currentCell, posToLnCol[previousPosition].Item1, commentsByLn.Keys.Max() + 1);
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
    
    private static void WriteDebug(bool isWriteDebug, StreamWriter output, string programCode, int position, Dictionary<int, Tuple<int, int>> posToLnCol, IList<char> memoryCells, int currentCell) {
        if (isWriteDebug) output.WriteLine($"code[{posToLnCol[position].Item1, 3}, {posToLnCol[position].Item2, 3}]: {programCode[position]} | cell[{currentCell,3}] = {(int)memoryCells[currentCell],5} ({ConvertToPrintable(memoryCells[currentCell])})");
    }

    private static IList<string> GetComments(Dictionary<int, string> commentsByLn, int fromLn, int toLn) {
        var printedComments = new List<string>();
        for (var i = fromLn; i < toLn; i++) {
            if (commentsByLn.TryGetValue(i, out var comment)) {
                printedComments.Add(comment);
            }
        }
        return printedComments;
    }

    private static void WriteComments(ref bool isWriteDebug, StreamReader input, StreamWriter output, Dictionary<int, string> commentsByLn, IList<char> memoryCells, int currentCell, int fromLn, int toLn) {
        var commentsToWrite = GetComments(commentsByLn, fromLn, toLn);
        foreach (var comment in commentsToWrite) {
            var lastStartDebuggingPos = -1;
            var lastStopDebuggingPos = -1;
            if (StartDebuggingRegex.IsMatch(comment)) {
                lastStartDebuggingPos = StartDebuggingRegex.Matches(comment).Max(x => x.Captures.Max(y => y.Index));
                isWriteDebug = true;
            }
            if (StartDebuggingParametrizedRegex.IsMatch(comment)) {                
                var matches = StartDebuggingParametrizedRegex.Matches(comment);
                foreach (Match match in matches) {
                    var cellToCheck = int.Parse(match.Groups[1].Value);
                    var expectedValue = int.Parse(match.Groups[2].Value);
                    if (memoryCells.Count > cellToCheck && memoryCells[cellToCheck] == expectedValue) {
                        lastStartDebuggingPos = Math.Max(lastStartDebuggingPos, match.Captures.Max(y => y.Index));
                        isWriteDebug = true;
                    }
                }
            }
            WriteComment(isWriteDebug, output, comment, memoryCells, currentCell);
            if (StopDebuggingRegex.IsMatch(comment)) {
                lastStopDebuggingPos = StopDebuggingRegex.Matches(comment).Max(x => x.Captures.Max(y => y.Index));
            }
            if (StopDebuggingParametrizedRegex.IsMatch(comment)) {
                var matches = StopDebuggingParametrizedRegex.Matches(comment);
                foreach (Match match in matches) {
                    var cellToCheck = int.Parse(match.Groups[1].Value);
                    var expectedValue = int.Parse(match.Groups[2].Value);
                    if (memoryCells.Count > cellToCheck && memoryCells[cellToCheck] == expectedValue) {
                        lastStopDebuggingPos = Math.Max(lastStopDebuggingPos, match.Captures.Max(y => y.Index));
                    }
                }
            }
            if (lastStartDebuggingPos > lastStopDebuggingPos) {
                isWriteDebug = true;
            }
            else if (lastStartDebuggingPos < lastStopDebuggingPos) {
                isWriteDebug = false;
            }
            if (BreakRegex.IsMatch(comment)) {
                output.WriteLine("Breakpoint is met. Press enter to continue execution...");
                output.Flush();
                input.ReadLine();
            }
            else if (BreakParametrizedRegex.IsMatch(comment)) {
                var match = BreakParametrizedRegex.Match(comment);
                var cellToCheck = int.Parse(match.Groups[1].Value);
                var expectedValue = int.Parse(match.Groups[2].Value);
                if (memoryCells.Count > cellToCheck && memoryCells[cellToCheck] == expectedValue) {
                    output.WriteLine($"Customized breakpoint {match.Groups[0]} is met. Press enter to continue execution...");
                    output.Flush();
                    input.ReadLine();
                }
            }
        }
    }

    private static void WriteComment(bool isWriteDebug, StreamWriter output, string comment, IList<char> memoryCells, int currentCell) {
        if (isWriteDebug) output.WriteLine($"{comment}");
        WriteMemoryCells(isWriteDebug, output, memoryCells, currentCell);
    }

    private static void WriteMemoryCells(bool isWriteDebug, StreamWriter output, IList<char> memoryCells, int currentCell) {
        if (!isWriteDebug) return;
        output.WriteLine($"|{string.Join("|", Enumerable.Range(0, memoryCells.Count).Select(x => x == currentCell ? @"\\/\/\/" : "       "))}|");
        output.WriteLine($"| {string.Join(" | ", memoryCells.Select(x => $"{(int)x,5}"))} |");
        output.WriteLine($"|   {string.Join("   |   ", memoryCells.Select(ConvertToPrintable))}   |");
    }
}
