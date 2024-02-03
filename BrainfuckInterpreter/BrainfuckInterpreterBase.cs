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
}
