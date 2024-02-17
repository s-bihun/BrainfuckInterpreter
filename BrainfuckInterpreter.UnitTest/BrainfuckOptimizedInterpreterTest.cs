global using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace BrainfuckInterpreter.UnitTest;

[TestClass]
public class BrainfuckOptimizedInterpreterTest
{
    private BrainfuckOptimizedInterpreter? Interpreter;

    [TestInitialize]
    public void TestInitialize()
    {
        Interpreter = new BrainfuckOptimizedInterpreter();
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
        Assert.AreEqual("Hello World!", new StreamReader(memoryStream).ReadToEnd());
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
        Assert.AreEqual("Shit Happens", new StreamReader(memoryStream).ReadToEnd());
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
        Assert.AreEqual("funny", new StreamReader(memoryStream).ReadToEnd());
    }

    [TestMethod]
    public void AddNumbersTest()
    {
        var program = ">+>>>++++++++++>>>,>++++++[<-------->-],>++++++[<-------->-]<[<+>-]<[<+<<<+>>>>-]<<<<[->-[>]<<]<[>++++++[<++++++++>-]<.>++++++[<-------->-]<->>>>>>++++++[<++++++>-]<++.<<<<<]<[-<>>>>>>>++++++[<++++++++>-]<.<<<<<<]";
        var intpuStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("98")));
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        Assert.AreEqual("17", new StreamReader(memoryStream).ReadToEnd());
    }

    [TestMethod]
    public void BubbleSortTest()
    {
        var program = ">>,[>>,]<<[[<<]>>>>[<<[>+<<+>-]>>[>+<<<<[->]>[<]>>-]<<<[[-]>>[>+<-]>>[<<<+>>>-]]>>[[<+>-]>>]<]<<[>>+<<-]<<]>>>>[.>>]";
        var intpuStream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes("3815024967\0")));
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        Assert.AreEqual("0123456789", new StreamReader(memoryStream).ReadToEnd());
    }

    [TestMethod]
    public void AccessingNegativeIndexMemoryCells()
    {
        
        var program = "<>";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // should throw out of memory
    }

    [TestMethod]
    public void RequestingTooManyMemory()
    {
        var program = $";{{STOP_DEBUGGING}}+[>+]";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // out of memory
    }

    [TestMethod]
    public void ProgramIsTooLong()
    {
        var program = Enumerable.Repeat('+', 30001).ToString();
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // program is too long
    }

    [TestMethod]
    public void LoopHasNoEnd()
    {
        var program = "[";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // loop has no end
    }

    [TestMethod]
    public void LoopHasNoStart()
    {
        var program = "]";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // loop has no start
    }

    [TestMethod]
    public void InputIsNull()
    {
        var program = ",";
        var intpuStream = (StreamReader)null;
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // null pointer
    }

    [TestMethod]
    public void InputStreamIsInaccessible()
    {
        var program = ",";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // meaningful exception
    }
    
    [TestMethod]
    public void OutputIsNull()
    {
        var program = ".";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = (StreamWriter)null;
        Interpreter?.Interpret(program, intpuStream, outputStream); // null pointer
    }

    [TestMethod]
    public void OutputStreamIsInaccessible()
    {
        var program = ".";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // meaningful exception
    }

    [TestMethod]
    public void UnexpectedExceptionShouldBeRethrown()
    {
        var program = "+";
        var intpuStream = new StreamReader(new MemoryStream());
        var memoryStream = new MemoryStream();
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream); // exception should be rethrown
    }

    // TODO: anything else in terms of exceptions?
}