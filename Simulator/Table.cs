namespace Simulator;

/*
 Syntax:
   Each line should contain one tuple of the form '<current state> <current symbol> <new symbol> <direction> <new state>'.
   You can use any number or word for <current state> and <new state>, eg. 10, a, state1. State labels are case-sensitive.
   You can use almost any character for <current symbol> and <new symbol>, or '_' to represent blank (space). Symbols are case-sensitive.
   You can't use ';', '*', '_' or whitespace as symbols.
   <direction> should be 'l', 'r' or '*', denoting 'move left', 'move right' or 'do not move', respectively.
   Anything after a ';' is a comment and is ignored.
   The machine halts when it reaches any state starting with 'halt', eg. halt, halt-accept.
 Also:
   '*' can be used as a wildcard in <current symbol> or <current state> to match any character or state.
   '*' can be used in <new symbol> or <new state> to mean 'no change'.
   '!' can be used at the end of a line to set a breakpoint, eg '1 a b r 2 !'. The machine will automatically pause after executing this line.
   You can specify the starting position for the head using '*' in the initial input.
https://morphett.info/turing/turing.html
 */

public class Table
{
    private readonly CodeMatch<string, string, string[]> codeDictionary;
    private readonly string comment = ";";
    private const char SEPARATOR = ' ';

    /// <param name="program">The code splitted by lines</param>
    public Table(string[] program)
    {
        codeDictionary = new CodeMatch<string, string, string[]>();

        foreach (var line in program)
        {
            if (line.StartsWith(comment))
                continue;

            var tmp = line.Split(SEPARATOR);
            if (tmp.Length != 5)
                continue;

            if (!codeDictionary.ContainsKey(tmp[0]))
            {
                codeDictionary.Add(tmp[0], new Dictionary<string, string[]>());
            }

            codeDictionary[tmp[0]].Add(tmp[1], tmp.Skip(2).ToArray());
        }
    }

    /// <param name="program">The code splitted by lines</param>
    /// <param name="comment">The comment delimiter to use</param>
    public Table(string[] program, string comment) : this(program)
    {
        this.comment = comment;
    }

    /// <summary>
    /// Matches the right command line to the actual program state
    /// </summary>
    /// <param name="state">The program state to match</param>
    /// <param name="symbol">The indexed tape symbol</param>
    /// <returns>The right command line</returns>
    public Instruction Match(string state, string symbol)
    {
        symbol = Symbol.Escape(symbol);

        if (!codeDictionary.TryGetValue(state, out var right))
            throw new KeyNotFoundException("Match not found in state '" + state + "' for symbol '" + symbol + "'");

        if (right.TryGetValue(symbol, out var cmd))
            return new Instruction(cmd);

        try
        {
            return new Instruction(right[Symbol.WILDCARD_IN_PROGRAM]);
        }
        catch (KeyNotFoundException)
        {
            throw new KeyNotFoundException("Match not found in state '" + state + "' for symbol '" + symbol + "'");
        }
    }
}

public class Symbol
{
    public const string BLANK_ON_TAPE = " ";
    public const string BLANK_IN_PROGRAM = "_"; //  '_' to represent blank (space) when matching
    public const string WILDCARD_IN_PROGRAM = "*";   //  wildcard

    public static string Escape(string symbol)
    {
        return symbol == BLANK_ON_TAPE ? BLANK_IN_PROGRAM : symbol;
    }
}

public class Instruction
{
    // operation types
    public const string NO_CHANGE = "*"; // not write anything, keep it.
    public const string ERASURE = "_";
    public const string MOVE_RIGHT = "r";
    public const string MOVE_LEFT = "l";
    public const string NO_MOVE = "*";

    // special symbols
    public const string SPACE = " ";

    private readonly string[] _command;

    public Instruction(string[] cmd)
    {
        if (cmd?.Length != 3)
        {
            throw new ArgumentException(nameof(cmd));
        }

        _command = cmd;
    }

    public void Execute(Head<char> head, State state)
    {
        if (IsWriteOrErase)
            head.Write(Convert.ToChar(IsErase ? SPACE : _command[0]));

        if (IsMoveRight)
            head.MoveRight();
        else if (IsMoveLeft)
            head.MoveLeft();

        state.Transition(_command[2]);
    }

    public bool IsWriteOrErase => _command[0] != NO_CHANGE;

    public bool IsErase => _command[0] == ERASURE;

    public bool IsMoveRight => _command[1] == MOVE_RIGHT;

    public bool IsMoveLeft => _command[1] == MOVE_LEFT;
}

/// <summary>
/// A two-key dictionary
/// </summary>
public class CodeMatch<K1, K2, V1> : Dictionary<K1, Dictionary<K2, V1>>
{
    public new Dictionary<K2, V1> this[K1 key]
    {
        get
        {
            if (!ContainsKey(key))
                Add(key, new Dictionary<K2, V1>());

            TryGetValue(key, out var returnObj);

            return returnObj;
        }
    }
}