using BrainfuckInterpreter;
                    
public class Program
{
    public static void Main(string[] args)
    {
        // args = [@"..\..\..\..\BrainfuckPrograms\ChristmassTree.bf", "-d"];
        if (args.Length < 1) throw new InvalidOperationException("Brainfuck program file should be passed as a second argument.");
        string brainfuckProgramFilePath = args[0];
        bool isDebugMode = args.Length == 2 && args[1] == "-d";

        var programFile = File.OpenRead(brainfuckProgramFilePath);
        var programCode = new StreamReader(programFile).ReadToEnd();
        var intpuStreamReader = new StreamReader(Console.OpenStandardInput());
        var outputStreamWriter = new StreamWriter(Console.OpenStandardOutput());
        BrainfuckInterpreterBase interpreter;
        if (isDebugMode) {
            interpreter = new BrainfuckDebugInterpreter();
        }
        else {
            interpreter = new BrainfuckOptimizedInterpreter();
            programCode = interpreter.Strip(programCode);
        }

        if (args.Length == 2 && args[1] == "-s") { // flag to stripline the program, not to execute it
            Console.WriteLine(interpreter.Strip(programCode));
        } else {
            interpreter.Interpret(programCode, intpuStreamReader, outputStreamWriter);
            outputStreamWriter.Flush();
        }
    }
}