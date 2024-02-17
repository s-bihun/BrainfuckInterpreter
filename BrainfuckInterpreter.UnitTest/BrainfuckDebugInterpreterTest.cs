global using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace BrainfuckInterpreter.UnitTest;

[TestClass]
public class BrainfuckDebugInterpreterTest
{
    private static readonly string DebugResultOutputPrefix = $"Program output:{Environment.NewLine}";

    private BrainfuckDebugInterpreter? Interpreter;

    [TestInitialize]
    public void TestInitialize()
    {
        Interpreter = new BrainfuckDebugInterpreter();
    }

    [TestMethod]
    public void HelloWorldTest()
    {
        var program = "++++++++++[>+++++++>++++++++++>+++<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        var debugResults = new StreamReader(memoryStream).ReadToEnd();
        var programOutput = debugResults.Substring(debugResults.IndexOf(DebugResultOutputPrefix) + DebugResultOutputPrefix.Length);
        Assert.AreEqual("Hello World!", programOutput);
    }

    [TestMethod]
    public void ShitHappensTest()
    {
        var program = "++++++++++[>+++>++++++++>++++++++++<<<-]" +
            ">>+++.>++++.+.+++++++++++." +
            "<<++." +
            ">-----------.>-------------------.+++++++++++++++..-----------.+++++++++.+++++.";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        var debugResults = new StreamReader(memoryStream).ReadToEnd();
        var programOutput = debugResults.Substring(debugResults.IndexOf(DebugResultOutputPrefix) + DebugResultOutputPrefix.Length);
        Assert.AreEqual("Shit Happens", programOutput);
    }

    [TestMethod]
    public void FunnyTest()
    {
        var program = ">++++++++++[>++++++++++>++++++++++++>+++++++++++<<<-]" +
            ">++>--->>+><<<<<" +
            ">.>.>..<++++.";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        var debugResults = new StreamReader(memoryStream).ReadToEnd();
        var programOutput = debugResults.Substring(debugResults.IndexOf(DebugResultOutputPrefix) + DebugResultOutputPrefix.Length);
        Assert.AreEqual("funny", programOutput);
    }

    [TestMethod]
    public void AddNumbersTest()
    {
        var program = ">+>>>++++++++++>>>,>++++++[<-------->-],>++++++[<-------->-]<[<+>-]<[<+<<<+>>>>-]<<<<[->-[>]<<]<[>++++++[<++++++++>-]<.>++++++[<-------->-]<->>>>>>++++++[<++++++>-]<++.<<<<<]<[-<>>>>>>>++++++[<++++++++>-]<.<<<<<<]";
        var intpuStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("9\r\n8")));
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        var debugResults = new StreamReader(memoryStream).ReadToEnd();
        var programOutput = debugResults.Substring(debugResults.IndexOf(DebugResultOutputPrefix) + DebugResultOutputPrefix.Length);
        Assert.AreEqual("17", programOutput);
    }

    [TestMethod]
    public void BubbleSortTest()
    {
        var program = $";{{STOP_DEBUGGING}}{Environment.NewLine}>>,[>>,]<<[[<<]>>>>[<<[>+<<+>-]>>[>+<<<<[->]>[<]>>-]<<<[[-]>>[>+<-]>>[<<<+>>>-]]>>[[<+>-]>>]<]<<[>>+<<-]<<]>>>>[.>>]";
        var intpuStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes($"3{Environment.NewLine}8{Environment.NewLine}1{Environment.NewLine}5{Environment.NewLine}0{Environment.NewLine}2{Environment.NewLine}4{Environment.NewLine}9{Environment.NewLine}6{Environment.NewLine}7{Environment.NewLine}\0")));
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        var debugResults = new StreamReader(memoryStream).ReadToEnd();
        var programOutput = debugResults.Substring(debugResults.IndexOf(DebugResultOutputPrefix) + DebugResultOutputPrefix.Length);
        Assert.AreEqual("0123456789", programOutput);
    }
}