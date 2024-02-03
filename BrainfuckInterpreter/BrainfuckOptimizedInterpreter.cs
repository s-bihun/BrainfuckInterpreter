namespace BrainfuckInterpreter;

/// <summary>
/// Optimized interpreter for the Brainfuck programs.
/// Interpret calls from different threads are not supported.
/// </summary>
public class BrainfuckOptimizedInterpreter : BrainfuckInterpreterBase {
    #region Constants

    private const int ExpectedEnclosedLoopNumber = 255;
    private const int ExpectedLoopNumber = 255;
    private const int AvailableMemoryCellsCount = 10240;

    #endregion

    /// <inheritdoc/>
    public override void Interpret(string programCode, StreamReader input, StreamWriter output) {
        var encounteredLoopStarts = new List<int>(ExpectedEnclosedLoopNumber);
        var currentLoopStartPosition = -1;
        var goToPositionsCache = new Dictionary<int, int>(2 * ExpectedLoopNumber);
        
        var programPos = 0;
        var memoryCells = Enumerable.Repeat(DefaultCellContent, AvailableMemoryCellsCount).ToList();
        int currentCell = 0;
        while (programPos < programCode.Length) {
            switch (programCode[programPos]) {
                case GoRightCommand:
                    currentCell++;
                    break;
                case GoLeftCommand:
                    currentCell--;
                    break;
                case IncrementCommand:
                    memoryCells[currentCell]++;
                    break;
                case DecrementCommand:
                    memoryCells[currentCell]--; 
                    break;
                case OutputCommand:
                    output.WriteAsync(memoryCells[currentCell]);
                    break;
                case InputCommand:
                    memoryCells[currentCell] = (char)input.Read();
                    break;
                case LoopStartCommand:
                    if (memoryCells[currentCell] == 0) {
                        if (goToPositionsCache.TryGetValue(programPos, out var newPosition)) {
                            programPos = newPosition;
                        }
                        else {
                            var oldPosition = programPos;
                            programPos = FindNextCommandAfterLoopPosition(programCode, programPos);
                            goToPositionsCache.Add(oldPosition, programPos);
                        }
                        continue;
                    }
                    else {
                        currentLoopStartPosition++;
                        if (encounteredLoopStarts.Count <= currentLoopStartPosition) {
                            encounteredLoopStarts.Add(programPos);
                        }
                        else {
                            encounteredLoopStarts[currentLoopStartPosition] = programPos;
                        }
                    }
                    break;
                case LoopEndCommand:
                    if (memoryCells[currentCell] != 0) {
                        if (goToPositionsCache.TryGetValue(programPos, out var newPosition)) {
                            programPos = newPosition;
                        }
                        else {
                            var oldPosition = programPos;
                            programPos = encounteredLoopStarts[currentLoopStartPosition--] + 1;
                            goToPositionsCache.Add(oldPosition, programPos);
                        }
                        continue;
                    }
                    break;
            }
            programPos++;
        }
    }
    
    /// <summary>
    /// Find position of the next command after the loop.
    /// </summary>
    /// <param name="code">Brainfuck code.</param>
    /// <param name="position">Current position (expected start of the loop).</param>
    /// <returns>Position of the next command after the loop.</returns>
    private static int FindNextCommandAfterLoopPosition(string code, int position) {
        var matchingBracket = LoopEndCommand;
        var opposingBracket = LoopStartCommand;
        int counter = 1;
        do {
            position++;
            //if (position < 0) throw new InvalidOperationException("Program is invalid. Loop is closed but no starting point.");
            if (code[position] == opposingBracket) { counter++; continue; }
            if (code[position] == matchingBracket) counter--;
        } while (!(counter == 0 && code[position] == matchingBracket));
        return position + 1;
    }
}