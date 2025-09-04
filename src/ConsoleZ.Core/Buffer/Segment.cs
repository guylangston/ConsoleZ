namespace ConsoleZ.Core.Buffer;

public interface ISegmentLayout<TClr> : IEnumerable<Segment<TClr>>
{
    IScreenBuffer<TClr> ParentBuffer { get; }
    int CellCount { get; }
}

public readonly struct Segment<TClr>
{
    public Segment(int x, int y, int idx, WindowScreenBuffer<TClr> buffer) : this()
    {
        X = x;
        Y = y;
        Idx = idx;
        Buffer = buffer;
    }

    public int X { get;  }
    public int Y { get;  }
    public int Idx { get;  }
    public WindowScreenBuffer<TClr> Buffer { get;  }
}


