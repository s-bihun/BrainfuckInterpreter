namespace BrainfuckInterpreter;

/// <summary>
/// Brainfuck program interpreter.
/// </summary>
public interface IBrainfuckInterpreter {
    /// <summary>
	/// Interpret Brainfuck program.
	/// </summary>
	/// <param name="programCode">Code of a program.</param>
	/// <param name="input">Input stream in form of a stream reader.</param>
	/// <param name="output">Output stream in form of a stream writer.</param>
	/// <exception cref="InvalidOperationException">Thrown on interpretation errors.</exception>
    public void Interpret(string programCode, StreamReader input, StreamWriter output);
}
