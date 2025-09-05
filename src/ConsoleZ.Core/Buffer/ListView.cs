

namespace ConsoleZ.Core.Buffer;

public class ListViewState
{
    public virtual int Offset { get; set; }
    public virtual int CursorWindowIdx { get; set; }

    public virtual void Reset()
    {
        Offset = 0;
        CursorWindowIdx = 0;
    }
}

public class ListView<TClr, TData>
{
    readonly ListViewState state;

    public ListView(ListViewState state)
    {
        this.state = state;
    }

    public IReadOnlyList<TData>? Data { get; private set;}
    public ISegmentLayout<TClr>? Layout { get; private set; }
    public int Offset { get => state.Offset; set => state.Offset = value; }
    public int CursorWindowIdx { get => state.CursorWindowIdx; set => state.CursorWindowIdx = value; }
    public int CursorDataIdx => Offset + CursorWindowIdx;

    public readonly struct ViewSegment
    {
        public Segment<TClr> Segment { get; init; }
        public TData Data { get; init; }
        public int DataIdx { get; init; }
        public bool IsCursor { get; init; }
    }

    public void Reset() => state.Reset();

    /// <returns>false if source reset</returns>
    public bool VerifySource(IReadOnlyList<TData>? data)
    {
        if (Data == null && data == null) return false; // nothing to do

        if (Data == null && data != null)
        {
            Data = data;
            state.Reset();
            return false;
        }

        if (Data != null && data == null)
        {
            Data = null;
            state.Reset();
            return false;
        }

        if (object.ReferenceEquals(Data, data)) return true;

        if (Data!.Count != data!.Count)
        {
            state.Reset();
            Data = data;
            return false;
        }

        Data = data;
        return false;
    }

    public void VerifyLayout(ISegmentLayout<TClr> layout)
    {
        // don't detect, just reassign
        this.Layout = layout;
    }

    public void First()
    {
        CursorWindowIdx = 0;
        Offset = 0;
    }

    public void Last()
    {
        if (Data == null || Layout == null) return;
        if (Data.Count > Layout.CellCount)
        {
            Offset = Data.Count - Layout.CellCount;
            CursorWindowIdx = Layout.CellCount - 1;
        }
    }

    public bool Next()
    {
        if (Data == null || Layout == null) return false;
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
        if (Data == null || Layout == null) return false;
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
        if (Data == null || Layout == null) return false;
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
        if (Data == null || Layout == null) return false;
        if (Layout is LayoutGrid<TClr> grid)
        {
            if (Offset + CursorWindowIdx + grid.Columns < Data.Count)
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
        if (Data == null || Layout == null) yield break;

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

