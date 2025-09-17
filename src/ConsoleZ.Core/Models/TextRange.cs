using System.Collections;

namespace ConsoleZ.Core.Models;

public struct TextRange : IEnumerable<int>
{
    public TextRange(int start, int end)
    {
        if (start <= end)
        {
            Start = start;
            End = end;
        }
        else
        {
            Start = end;
            End = start;
        }
    }

    public static TextRange CreateEmpty() => new TextRange(-1, -1);

    public int Start { get; set; }

    /// <summary>Inclusive End</summary>
    public int End { get; set;}

    public void SetEmpty() => Start = End = -1;

    public readonly bool IsEmpty => Start < 0;

    public readonly int Length => End - Start + 1;

    public IEnumerator<int> GetEnumerator()
    {
        if (!IsEmpty)
        {
            for(var cc=Start; cc<=End; cc++)
            {
                yield return cc;
            }
        }
    }
    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}

