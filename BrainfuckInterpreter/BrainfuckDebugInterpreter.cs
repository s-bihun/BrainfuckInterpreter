﻿using System.Text;
using System.Text.RegularExpressions;

namespace BrainfuckInterpreter;

/// <summary>
/// Brainfuck interpreter supporting comments, brakepoints and debug outputs.
/// Supports only single-threaded usage.
/// </summary>
public class BrainfuckDebugInterpreter : BrainfuckInterpreterBase {
    #region Constants

    private const int StopEachExecutedCommandCount = 1000;
    private const int ShowMemoryCellsBothSidesDefault = 10;

    #endregion

    #region Regexes

    private static readonly Regex StopDebuggingRegex = new Regex(@"\{STOP_DEBUGGING\}");
    private static readonly Regex StopDebuggingParametrizedRegex = new Regex(@"\{STOP_DEBUGGING\(CELL\[(\d+)\]==(\d+)\)\}");
    private static readonly Regex StartDebuggingRegex = new Regex(@"\{START_DEBUGGING\}");
    private static readonly Regex StartDebuggingParametrizedRegex = new Regex(@"\{START_DEBUGGING\(CELL\[(\d+)\]==(\d+)\)\}");
    private static readonly Regex BreakRegex = new Regex(@"\{BREAK\}");
    private static readonly Regex BreakParametrizedRegex = new Regex(@"\{BREAK\(CELL\[(\d+)\]==(\d+)\)\}");

    #endregion

    #region Private Fields

    private string? StrippedCode;
    private bool IsWriteDebug;
    private List<byte>? MemoryCells;
    private int CurrentCell;
    private int Position;
    private StreamReader? Input;
    private StreamWriter? Output;
    private Dictionary<int, Tuple<int, int>>? PosToLnCol;
    private Dictionary<int, string>? CommentsByLn;

    #endregion

    /// <inheritdoc/>
    public override void Interpret(string programCode, StreamReader input, StreamWriter output) {
        int commandsExecutedCounter = 0;
        Input = input;
        Output = output;
        var programOutput = new StringBuilder();
        StrippedCode = Strip(programCode, out PosToLnCol, out CommentsByLn);
        var foundLoopStartPositions = new List<int>();
        var currentLoopStartPosition = -1;
        var foundGoToPositions = new Dictionary<int, int>();

        IsWriteDebug = true;
        MemoryCells = new List<byte>() { DefaultCellContent };
        CurrentCell = 0;
        Position = 0;
        var previousPosition = -1;
        while (Position < StrippedCode.Length) {
            if ((previousPosition < 0 && PosToLnCol[Position].Item1 != 0) || (PosToLnCol[previousPosition].Item1 != PosToLnCol[Position].Item1)) {
                WriteCommentsForLines(previousPosition < 0 ? 0 : PosToLnCol[previousPosition].Item1, PosToLnCol[Position].Item1);
            }
            previousPosition = Position;

            switch (StrippedCode[Position]) {
                case GoRightCommand:
                    CurrentCell++;
                    if (MemoryCells.Count < (CurrentCell + 1)) MemoryCells.Add(DefaultCellContent);
                    break;
                case GoLeftCommand:
                    CurrentCell--;
                    if (CurrentCell < 0) throw new InvalidOperationException("Invalid program. Memory cell with index less then zero is referenced.");
                    break;
                case IncrementCommand:
                    MemoryCells[CurrentCell]++;
                    break;
                case DecrementCommand:
                    MemoryCells[CurrentCell]--;
                    break;
                case OutputCommand:
                    programOutput.Append((char)MemoryCells[CurrentCell]);
                    break;
                case InputCommand:
                    WriteDebugMessage(false);
                    try {
                        MemoryCells[CurrentCell] = (byte)(Input.ReadLine()?.First() ?? '\n');
                    }
                    catch (InvalidOperationException) {
                        MemoryCells[CurrentCell] = (byte)'\n';
                    }
                    break;
                case LoopStartCommand:
                    WriteDebugMessage();
                    if (MemoryCells[CurrentCell] == 0) {
                        if (!foundGoToPositions.TryGetValue(Position, out var loopEndPosition)) {
                            loopEndPosition = FindEndOfLoopPosition(StrippedCode, Position);
                            foundGoToPositions.Add(Position, loopEndPosition);
                            foundGoToPositions.Add(loopEndPosition, Position);
                        }
                        Position = loopEndPosition;
                        previousPosition = Position;
                    }
                    else {
                        if (!foundGoToPositions.ContainsKey(Position)) { // do not add start of the loop to foundLoopStartPositions if we're entering loop in the other loop iteration
                            if (foundLoopStartPositions.Count <= ++currentLoopStartPosition) {
                                foundLoopStartPositions.Add(Position);
                            }
                            else {
                                foundLoopStartPositions[currentLoopStartPosition] = Position;
                            }
                        }
                    }
                    Position++;
                    continue;
                case LoopEndCommand:
                    WriteDebugMessage();
                    if (!foundGoToPositions.TryGetValue(Position, out var loopStartPosition)) {
                        loopStartPosition = foundLoopStartPositions[currentLoopStartPosition--];
                        foundGoToPositions.Add(Position, loopStartPosition);
                        foundGoToPositions.Add(loopStartPosition, Position);
                    }
                    if (MemoryCells[CurrentCell] != 0) {
                        Position = loopStartPosition;
                        previousPosition = Position;
                    }
                    Position++;
                    continue;
                default:
                    throw new InvalidOperationException($"Command code[{Position}] = \"{StrippedCode[Position]}\" is not defined.");
            }
            WriteDebugMessage();
            Position++;

            commandsExecutedCounter++;
            if (commandsExecutedCounter % StopEachExecutedCommandCount == 0 && IsWriteDebug) {
                Output.WriteLine($"Current output:\n{programOutput}");
                Output.WriteLine($"Check if there'is no endless loop and continue by pressing Enter (occurs each {StopEachExecutedCommandCount} executed commands)...");
                Output.Flush();
                Input.ReadLine();
            }
        }
        WriteCommentsForLines(PosToLnCol[previousPosition].Item1, (CommentsByLn.Keys.Any() ? CommentsByLn.Keys.Max() : 0) + 1);
        Output.WriteLine();
        Output.WriteLine($"Total program length:    {Position,10}");
        Output.WriteLine($"Total commands executed: {commandsExecutedCounter,10}");
        Output.WriteLine($"Total memory cells used: {MemoryCells.Count,10}");
        Output.WriteLine($"Memory content:");
        IsWriteDebug = true;
        WriteMemoryCells();
        Output.WriteLine();
        Output.WriteLine("Program output:");
        Output.Write(programOutput.ToString());
    }

    private static char ConvertToPrintable(byte c) => c < 32 || c == 255 ? '' : (char)c;
    
    private static int FindEndOfLoopPosition(string code, int position) {
        var matchingBracket = LoopEndCommand;
        var opposingBracket = LoopStartCommand;
        int counter = 1;
        do {
            position++;
            if (position < 0) throw new InvalidOperationException("Program is invalid. Loop is closed but no starting point.");
            if (code[position] == opposingBracket) { counter++; continue; }
            if (code[position] == matchingBracket) counter--;
        } while (!(counter == 0 && code[position] == matchingBracket));
        return position;
    }
    
    private void WriteDebugMessage(bool includeCurrentCellValue = true) {
        if (Output == null) throw new InvalidOperationException($"{nameof(Output)} can't be null.");
        if (PosToLnCol == null) throw new InvalidOperationException($"{nameof(PosToLnCol)} can't be null.");
        if (StrippedCode == null) throw new InvalidOperationException($"{nameof(StrippedCode)} can't be null.");
        if (MemoryCells == null) throw new InvalidOperationException($"{nameof(MemoryCells)} can't be null.");

        if (IsWriteDebug) {
            var currentCellValue = includeCurrentCellValue ? $" | cell[{CurrentCell,3}] = {(int)MemoryCells[CurrentCell],5} ({ConvertToPrintable(MemoryCells[CurrentCell])})" : string.Empty;
            Output.WriteLine($"code[{PosToLnCol[Position].Item1, 3}, {PosToLnCol[Position].Item2, 3}]: {StrippedCode[Position]}{currentCellValue}");
            Output.Flush();
        }
    }

    private IList<string> GetComments(int fromLn, int toLn) {
        if (CommentsByLn == null) throw new InvalidOperationException($"{nameof(CommentsByLn)} can't be null.");

        var printedComments = new List<string>();
        for (var i = fromLn; i < toLn; i++) {
            if (CommentsByLn.TryGetValue(i, out var comment)) {
                printedComments.Add(comment);
            }
        }
        return printedComments;
    }

    private void WriteCommentsForLines(int fromLn, int toLn) {
        if (Output == null) throw new InvalidOperationException($"{nameof(Output)} can't be null.");
        if (Input == null) throw new InvalidOperationException($"{nameof(Input)} can't be null.");
        if (MemoryCells == null) throw new InvalidOperationException($"{nameof(MemoryCells)} can't be null.");

        var commentsToWrite = GetComments(fromLn, toLn);
        foreach (var comment in commentsToWrite) {
            var lastStartDebuggingPos = -1;
            var lastStopDebuggingPos = -1;
            if (StartDebuggingRegex.IsMatch(comment)) {
                lastStartDebuggingPos = StartDebuggingRegex.Matches(comment).Max(x => x.Captures.Max(y => y.Index));
                IsWriteDebug = true;
            }
            if (StartDebuggingParametrizedRegex.IsMatch(comment)) {                
                var matches = StartDebuggingParametrizedRegex.Matches(comment);
                foreach (Match match in matches) {
                    var cellToCheck = int.Parse(match.Groups[1].Value);
                    var expectedValue = int.Parse(match.Groups[2].Value);
                    if (MemoryCells.Count > cellToCheck && MemoryCells[cellToCheck] == expectedValue) {
                        lastStartDebuggingPos = Math.Max(lastStartDebuggingPos, match.Captures.Max(y => y.Index));
                        IsWriteDebug = true;
                    }
                }
            }
            WriteComment(comment, ShowMemoryCellsBothSidesDefault);
            if (StopDebuggingRegex.IsMatch(comment)) {
                lastStopDebuggingPos = StopDebuggingRegex.Matches(comment).Max(x => x.Captures.Max(y => y.Index));
            }
            if (StopDebuggingParametrizedRegex.IsMatch(comment)) {
                var matches = StopDebuggingParametrizedRegex.Matches(comment);
                foreach (Match match in matches) {
                    var cellToCheck = int.Parse(match.Groups[1].Value);
                    var expectedValue = int.Parse(match.Groups[2].Value);
                    if (MemoryCells.Count > cellToCheck && MemoryCells[cellToCheck] == expectedValue) {
                        lastStopDebuggingPos = Math.Max(lastStopDebuggingPos, match.Captures.Max(y => y.Index));
                    }
                }
            }
            if (lastStartDebuggingPos > lastStopDebuggingPos) {
                IsWriteDebug = true;
            }
            else if (lastStartDebuggingPos < lastStopDebuggingPos) {
                IsWriteDebug = false;
            }
            if (BreakRegex.IsMatch(comment)) {
                Output.WriteLine("Breakpoint is met. Press enter to continue execution...");
                Output.Flush();
                Input.ReadLine();
            }
            else if (BreakParametrizedRegex.IsMatch(comment)) {
                var match = BreakParametrizedRegex.Match(comment);
                var cellToCheck = int.Parse(match.Groups[1].Value);
                var expectedValue = int.Parse(match.Groups[2].Value);
                if (MemoryCells.Count > cellToCheck && MemoryCells[cellToCheck] == expectedValue) {
                    Output.WriteLine($"Customized breakpoint {match.Groups[0]} is met. Press enter to continue execution...");
                    Output.Flush();
                    Input.ReadLine();
                }
            }
        }
    }

    private void WriteComment(string comment, int showMemoryCellsBothSides = -1) {
        if (Output == null) throw new InvalidOperationException($"{nameof(Output)} can't be null.");

        if (IsWriteDebug) {
            Output.WriteLine($"{comment}");
            WriteMemoryCells(showMemoryCellsBothSides);
            Output.Flush();
        }
    }

    private void WriteMemoryCells(int showMemoryCellsBothSides = -1) {
        if (Output == null) throw new InvalidOperationException($"{nameof(Output)} can't be null.");
        if (MemoryCells == null) throw new InvalidOperationException($"{nameof(MemoryCells)} can't be null.");

        if (!IsWriteDebug) return;
        var showCellFrom = 0;
        var showCellCount = MemoryCells.Count;
        if (showMemoryCellsBothSides >= 0) {
            showCellFrom = Math.Max(CurrentCell - showMemoryCellsBothSides, 0);
            showCellCount = 2 * showMemoryCellsBothSides + 1;
            if (showCellFrom + showCellCount > MemoryCells.Count) {
                showCellCount = MemoryCells.Count - showCellFrom;
            }
        }
        var cellsToShow = MemoryCells.Skip(showCellFrom).Take(showCellCount);
        Output.WriteLine($"|{string.Join("|", Enumerable.Range(showCellFrom, showCellCount).Select(x => x == CurrentCell ? @"\\/\/\/" : $" {x,5} "))}|");
        Output.WriteLine($"| {string.Join(" | ", cellsToShow.Select(x => $"{(int)x,5}"))} |");
        Output.WriteLine($"|   {string.Join("   |   ", cellsToShow.Select(ConvertToPrintable))}   |");
    }
}
