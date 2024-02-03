using System.Text;
using System.Collections.Generic;
using System.IO;
namespace BrainfuckInterpreter;

/// <summary>
/// Optimized interpreter for the Brainfuck programs.
/// Interpret calls from different threads are not supported.
/// </summary>
public class BrainfuckOptimizedInterpreter : IBrainfuckInterpreter {
    #region Constants

    private const char GoRightCommand = '>';
    private const char GoLeftCommand = '<';
    private const char IncrementCommand = '+';   
    private const char DecrementCommand = '-';
    private const char OutputCommand = '.';
    private const char InputCommand = ',';
    private const char LoopStartCommand = '[';
    private const char LoopEndCommand = ']';
	private const char DefaultCellContent = (char)0;
    private const char LineCommentStartExtensionCommand = ';';
	private readonly char[] AllCommands = new [] { GoRightCommand, GoLeftCommand, IncrementCommand, DecrementCommand, OutputCommand, InputCommand, LoopStartCommand, LoopEndCommand };

    #endregion

	/// <inheritdoc/>
    public void Interpret(string programCode, StreamReader input, StreamWriter output) {
		var foundLoopStartPositions = new List<int>();
		var currentLoopStartPosition = -1;
        var foundGoToPositions = new Dictionary<int, int>();

		var memoryCells = new List<char>() { DefaultCellContent };
		int currentCell = 0;
		var position = 0;
		while (position < programCode.Length) {
			switch (programCode[position]) {
				case GoRightCommand:
					currentCell++;
					if (memoryCells.Count < (currentCell + 1)) memoryCells.Add(DefaultCellContent);
					break;
				case GoLeftCommand:
					currentCell--;
					//if (currentCell < 0) throw new InvalidOperationException("Invalid program. Memory cell with index less then zero is referenced.");
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
                        if (foundGoToPositions.TryGetValue(position, out var newPosition)) {
                            position = newPosition;
                        }
                        else {
							var oldPosition = position;
						    position = FindNextCommandAfterLoopPosition(programCode, position);
                            foundGoToPositions.Add(oldPosition, position);
                        }
						continue;
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
					break;
				case LoopEndCommand:
					if (memoryCells[currentCell] != 0) {
                        if (foundGoToPositions.TryGetValue(position, out var newPosition)) {
                            position = newPosition;
                        }
                        else {
							var oldPosition = position;
						    position = foundLoopStartPositions[currentLoopStartPosition--] + 1;
                            foundGoToPositions.Add(oldPosition, position);
                        }
						continue;
					}
					break;
				// default:
				//  throw new InvalidOperationException($"Command code[{position}] = \"{programCode[position]}\" is not defined.");
			}
			position++;
		}
	}

	/// <summary>
	/// Strip a Brainfuck program from comments and unsupported symbols.
	/// </summary>
	/// <param name="programCode">Code of a Brainfuck program to strip.</param>
	/// <returns>Stripped program.</returns>
	public string Strip(string programCode) {
		var strippedProgram = new StringBuilder();
		for(int i = 0; i < programCode.Length; i++) {
			if (programCode[i] == LineCommentStartExtensionCommand) {
				do {
					i++;
				} while (i < programCode.Length && programCode[i] != '\n');
				continue;
			}
			if (AllCommands.Contains(programCode[i])) strippedProgram.Append(programCode[i]);
		}
		return strippedProgram.ToString();
	}
	
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