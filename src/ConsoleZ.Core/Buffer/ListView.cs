namespace ConsoleZ.Core.Buffer;

public class ListView<TClr, TData>
{

    public ListView(IReadOnlyList<TData> data, ILayout<TClr> layout)
    {
        Data = data;
        Layout = layout;
        Offset = 0;
        CursorWindowIdx = 0;
    }

    public IReadOnlyList<TData> Data { get; }
    public ILayout<TClr> Layout { get; }
    public int Offset { get; set; }
    public int CursorWindowIdx { get; set; }
    public int CursorDataIdx => Offset + CursorWindowIdx;

    public readonly struct ViewSegment
    {
        public Segment<TClr> Segment { get; init; }
        public TData Data { get; init; }
        public int DataIdx { get; init; }
        public bool IsCursor { get; init; }
    }

    public void First()
    {
        CursorWindowIdx = 0;
        Offset = 0;
    }

    public void Last()
    {
        if (Data.Count > Layout.CellCount)
        {
            Offset = Data.Count - Layout.CellCount;
            CursorWindowIdx = Layout.CellCount - 1;
        }
    }

    public bool Next()
    {
        if (CursorDataIdx < Data.Count-1)
        {
            CursorWindowIdx++;
            if (CursorWindowIdx >= Layout.CellCount)
            {
                if (Layout is LayoutGrid<TClr> grid)
                {
                    Offset +=  grid.Columns;
                    CursorWindowIdx = Layout.CellCount - grid.Columns;
                    return true;
                }
                Offset++;
                CursorWindowIdx = Layout.CellCount-1;
            }
            return true;
        }
        return false;
    }

    public bool Previous()
    {
        if (CursorWindowIdx >= 0)
        {
            CursorWindowIdx--;
            if (CursorWindowIdx < 0)
            {
                if (Layout is LayoutGrid<TClr> grid)
                {
                    if (Offset >= grid.Columns)
                    {
                        Offset -=  grid.Columns;
                        CursorWindowIdx = 0;
                        return true;
                    }
                }
                CursorWindowIdx = 0;
                Offset--;
                if (Offset < 0) Offset = 0;
            }
            return true;
        }
        return false;
    }

    public bool MoveRight() => Next();
    public bool MoveLeft() => Previous();

    public bool MoveUp()
    {
        if (Layout is LayoutGrid<TClr> grid)
        {
            if (CursorWindowIdx - grid.Columns >= 0)
            {
                CursorWindowIdx -= grid.Columns;
                return true;
            }
            if (Offset - grid.Columns >= 0)
            {
                Offset -= grid.Columns;
                return true;
            }
            return false;
        }
        return Previous();
    }

    public bool MoveDown()
    {
        if (Layout is LayoutGrid<TClr> grid)
        {
            if (Offset + grid.Columns < Data.Count)
            {
                if (CursorWindowIdx + grid.Columns < Layout.CellCount)
                {
                    CursorWindowIdx += grid.Columns;
                }
                else
                {
                    Offset +=  grid.Columns;
                }

                return true;
            }
            return  false;
        }
        return Next();
    }

    public IEnumerable<ViewSegment> GetViewData()
    {
        var dataIdx = Offset;
        foreach(var segment in Layout)
        {
            if (dataIdx >= Data.Count) yield break;
            yield return new ViewSegment
            {
                Segment = segment,
                Data = Data[dataIdx],
                DataIdx = dataIdx,
                IsCursor = dataIdx == CursorDataIdx
            };
            dataIdx++;
        }
    }

}

