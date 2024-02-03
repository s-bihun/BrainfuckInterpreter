using System.Text;

namespace BrainfuckInterpreter;

/// <summary>
/// Brainfuck program interpreter.
/// </summary>
public abstract class BrainfuckInterpreterBase {
    #region Constants

    protected const char GoRightCommand = '>';
    protected const char GoLeftCommand = '<';
    protected const char IncrementCommand = '+';   
    protected const char DecrementCommand = '-';
    protected const char OutputCommand = '.';
    protected const char InputCommand = ',';
    protected const char LoopStartCommand = '[';
    protected const char LoopEndCommand = ']';
    protected const char DefaultCellContent = (char)0;
    protected const char LineCommentStartExtensionCommand = ';';
    protected const char NewLineChar = '\n';
    protected readonly char[] AllCommands = [GoRightCommand, GoLeftCommand, IncrementCommand, DecrementCommand, OutputCommand, InputCommand, LoopStartCommand, LoopEndCommand];

    #endregion
    /// <summary>
    /// Interpret Brainfuck program.
    /// </summary>
    /// <param name="programCode">Code of a program.</param>
    /// <param name="input">Input stream in form of a stream reader.</param>
    /// <param name="output">Output stream in form of a stream writer.</param>
    /// <exception cref="InvalidOperationException">Thrown on interpretation errors.</exception>
    public abstract void Interpret(string programCode, StreamReader input, StreamWriter output);

    /// <summary>
    /// Strip a Brainfuck program from comments and unsupported symbols.
    /// </summary>
    /// <param name="programCode">Code of a Brainfuck program to strip.</param>
    /// <returns>Stripped program.</returns>
    public string Strip(string programCode) => Strip(programCode, out _, out _);

    /// <summary>
    /// Strip a Brainfuck program from comments and unsupported symbols.
    /// </summary>
    /// <param name="programCode">Code of a Brainfuck program to strip.</param>
    /// <param name="strippedPositionToOriginalLnCol">Dictionary of stripped code positions to original code line & row tuples.
    /// <returns>Stripped program.</returns>
    protected string Strip(string programCode, out Dictionary<int, Tuple<int, int>> strippedPositionToOriginalLnCol, out Dictionary<int, string> commentsByLn) {      
        strippedPositionToOriginalLnCol = new Dictionary<int, Tuple<int, int>>();
        commentsByLn = new Dictionary<int, string>();
        int line = 1;
        int column = 1;
        int position = 0;
        var strippedProgram = new StringBuilder();
        for(int i = 0; i < programCode.Length; i++) {
            if (programCode[i] == LineCommentStartExtensionCommand) {
                var commentStartPos = i;
                do {
                    i++;
                } while (i < programCode.Length && programCode[i] != NewLineChar);
                commentsByLn.Add(line, programCode.Substring(commentStartPos + 1, i - commentStartPos - 1).Trim());
            }
            else {
                if (AllCommands.Contains(programCode[i])) {
                    strippedProgram.Append(programCode[i]);
                    strippedPositionToOriginalLnCol.Add(position++, new Tuple<int, int>(line, column));
                }
            }

            if (i < programCode.Length && programCode[i] == NewLineChar) {
                line++;
                column = 1;
            }
            else {
                column++;
            }
        }
        return strippedProgram.ToString();
    }
}
