using System.Collections;

namespace ConsoleZ.Core.Buffer;

public class LayoutGrid<TClr> : ISegmentLayout<TClr>
{
    public LayoutGrid(IScreenBuffer<TClr> buffer, int columns, int rows)
    {
        if (columns <= 0 || rows <= 0) throw new InvalidDataException();
        Buffer = buffer;
        Columns = columns;
        Rows = rows;

        CalcCellSize();
    }

    private void CalcCellSize()
    {
        CellWidth = Buffer.Width / Columns;
        CellHeight = Buffer.Height / Rows;

        if (CellWidth == 0 || CellHeight == 0) throw new InvalidDataException();

    }

    IScreenBuffer<TClr> ISegmentLayout<TClr>.ParentBuffer => Buffer;
    public IScreenBuffer<TClr> Buffer { get; }
    public int Columns { get; }
    public int Rows    { get; }
    public int CellWidth { get;  private set; }
    public int CellHeight { get; private set;  }
    public int CellCount => Rows * Columns;


    public IEnumerator<Segment<TClr>> GetEnumerator()
    {
        CalcCellSize();

        int cc = 0;
        for( int y=0; y<Rows; y++)
            for(int x=0; x< Columns; x++)
            {
                var buf = WindowBuffer.FromBuffer(Buffer, x * CellWidth, y * CellHeight, CellWidth, CellHeight);
                yield return new Segment<TClr>(x, y, cc++, buf);
            }

    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

