global using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrainfuckInterpreter.UnitTest;

[TestClass]
public class BrainfuckInterpreterTest
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
        var memoryStream = new MemoryStream(12);
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
        var memoryStream = new MemoryStream(12);
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
        var memoryStream = new MemoryStream(12);
        var outputStream = new StreamWriter(memoryStream);
        Interpreter?.Interpret(program, intpuStream, outputStream);
        outputStream.Flush();
        memoryStream.Position = 0;
        Assert.AreEqual("funny", new StreamReader(memoryStream).ReadToEnd());
    }
}