namespace BrainfuckInterpreter;

/// <summary>
/// Optimized interpreter for the Brainfuck programs.
/// Supports multy-thread usage.
/// </summary>
public class BrainfuckOptimizedInterpreter : BrainfuckInterpreterBase {
    #region Constants

    private const int ExpectedLoopNumber = 10000;
    private const int MaximumProgramLength = 30000;
    private const int AvailableMemoryCellsCount = 30000;

    #endregion

    /// <inheritdoc/>
    public override void Interpret(string programCode, StreamReader input, StreamWriter output) {
        var encounteredLoopStarts = new int[ExpectedLoopNumber];
        var currentLoopStartPosition = -1;
        var goToPositionsCache = Enumerable.Repeat(-1, MaximumProgramLength).ToArray();
        
        var programPos = 0;
        var memoryCells = new byte[AvailableMemoryCellsCount];
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
                    output.Write((char)memoryCells[currentCell]);
                    break;
                case InputCommand:
                    memoryCells[currentCell] = (byte)input.Read();
                    break;
                case LoopStartCommand:
                    if (memoryCells[currentCell] == 0) {
                        var loopEndPosition = goToPositionsCache[programPos];
                        if (loopEndPosition < 0) {
                            loopEndPosition = FindLoopEndPosition(programCode, programPos);
                            goToPositionsCache[programPos] = loopEndPosition;
                            goToPositionsCache[loopEndPosition] = programPos;
                        }
                        programPos = loopEndPosition;
                    }
                    else {
                        if (goToPositionsCache[programPos] < 0) {
                            encounteredLoopStarts[++currentLoopStartPosition] = programPos;
                        }
                    }
                    break;
                case LoopEndCommand:
                    var loopStartPosition = goToPositionsCache[programPos];
                    if (loopStartPosition < 0) {
                        loopStartPosition = encounteredLoopStarts[currentLoopStartPosition--];
                        goToPositionsCache[programPos] = loopStartPosition;
                        goToPositionsCache[loopStartPosition] = programPos;
                    }
                    if (memoryCells[currentCell] != 0) {
                        programPos = loopStartPosition;
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
    private static int FindLoopEndPosition(string code, int position) {
        var matchingBracket = LoopEndCommand;
        var opposingBracket = LoopStartCommand;
        int counter = 1;
        do {
            position++;
            if (code[position] == opposingBracket) { counter++; continue; }
            if (code[position] == matchingBracket) counter--;
        } while (!(counter == 0 && code[position] == matchingBracket));
        return position;
    }
}