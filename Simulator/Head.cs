using System.Collections;

namespace Simulator;

/// <summary>
/// A head that can read and write symbols on the tape and move the tape left and right one (and only one) cell at a time.
/// </summary>
public class Head
{
    private readonly Tape _tape;
    private int _index;
    private readonly char _emptyCell = Symbol.BLANK_ON_TAPE[0]; // The empty cell state. Used when expanding tape

    /// <summary>
    /// write and read on paper tape
    /// </summary>
    /// <param name="tape"></param>
    public Head(Tape tape)
    {
        _index = 0;
        _tape = tape;
    }

    /// <summary>
    /// Moves the _index to the right.
    /// </summary>
    /// <returns>The current cell</returns>
    public void MoveRight()
    {
        if (_index == _tape.Count - 1)
            _tape.Add(_emptyCell);

        _index++;
    }

    /// <summary>
    /// Moves the _index to the left
    /// </summary>
    /// <returns>The current cell</returns>
    public void MoveLeft()
    {
        if (_index == 0)
            _tape.Insert(0, _emptyCell);
        else
            _index--;
    }

    public char Read() => _tape[_index];

    public void Write(char item)
    {
        _tape[_index] = item;
    }
}

public class Tape : IEnumerable
{
    private readonly List<char> _cells;

    /// <summary>
    /// tape
    /// </summary>
    /// <param name="symbols">The initial tape state</param>
    public Tape(List<char> symbols)
    {
        _cells = symbols;
    }

    public int Count => _cells.Count;

    public char this[int index]
    {
        get => _cells[index];
        set => _cells[index] = value;
    }

    public void Add(char item)
    {
        _cells.Add(item);
    }

    public void Insert(int index, char item)
    {
        _cells.Insert(index, item);
    }

    public override string ToString() => string.Join(string.Empty, _cells);

    IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
}