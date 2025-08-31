using System.Collections;

namespace ConsoleZ.Core.Buffer;

public enum Orientation { Vert, Horz }
public class LayoutStack<TClr> : ILayout<TClr>
{
    public LayoutStack(IScreenBuffer<TClr> buffer, Orientation orientation, int count)
    {
        Buffer = buffer;
        Orientation = orientation;
        Count = count;

        CalcCellSize();
    }

    public IScreenBuffer<TClr> Buffer { get; }
    public Orientation Orientation { get; }
    public int Count { get; }

    public int CellWidth { get;  private set; }
    public int CellHeight { get; private set;  }
    public int CellCount => Count;

    private void CalcCellSize()
    {
        if (Orientation == Orientation.Vert)
        {
            CellWidth = Buffer.Width;
            CellHeight = Buffer.Height / Count;
        }
        else
        {
            CellWidth = Buffer.Width / Count;
            CellHeight = Buffer.Height;
        }
        if (CellWidth == 0 || CellHeight == 0) throw new InvalidDataException();
    }

    public IEnumerator<Segment<TClr>> GetEnumerator()
    {
        CalcCellSize();

        for(int cc=0; cc<Count; cc++)
        {
            if (Orientation == Orientation.Vert)
            {
                var buf = WindowBuffer.FromBuffer(Buffer, 0, cc * CellHeight, CellWidth, CellHeight);
                yield return new Segment<TClr>(0, cc, cc, buf);
            }
            else
            {
                var buf = WindowBuffer.FromBuffer(Buffer, cc * CellWidth, 0,  CellWidth, CellHeight);
                yield return new Segment<TClr>( cc, 0, cc, buf);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}


