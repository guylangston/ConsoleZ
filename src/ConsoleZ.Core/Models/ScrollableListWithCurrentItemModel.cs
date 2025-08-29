namespace ConsoleZ.Core.Models;

public class ScrollableListWithCurrentItemModel : ScrollableListModel
{
    int cursorLine;

    public ScrollableListWithCurrentItemModel(int windowSize, int listSize) : base(windowSize, listSize)
    {
    }

    public int CurrentItemIndex => Offset + cursorLine;

    public int LastLine => WindowSize-1;

    public bool TryGetWindowItemIndex(int windowIdx, out int itemIdex)
    {
        var idx = Offset + windowIdx;
        if (ContainsIndex(idx))
        {
            itemIdex = idx;
            return true;
        }

        itemIdex = -1;
        return false;
    }

    public bool ContainsIndex(int idx) => idx >= 0 && idx < ListSize;

    public override bool Previous()
    {
        if (ListSize < WindowSize) return false;

        if (cursorLine == 0) return base.Previous();
        if (cursorLine >= 0)
        {
            cursorLine--;
            return true;
        }
        return false;
    }
    public override bool Next()
    {
        if (ListSize < WindowSize) return false;
        if (cursorLine < LastLine)
        {
            cursorLine++;
            return true;
        }
        if (cursorLine >= LastLine)
        {
            return base.Next();
        }
        return false;
    }

    public override void Last()
    {
        if (ListSize < WindowSize) return;
        base.Last();
        cursorLine = (ListSize - Offset - 1);
    }

    public override void First()
    {
        if (ListSize < WindowSize) return;
        cursorLine = 0;
        base.First();
    }

    public (bool IsCurrent, T Data) GetWindowItemsWithCurrent<T>(IReadOnlyList<T> items, int relIdx)
    {
        var abs = Offset + relIdx;
        return (abs == CurrentItemIndex, items[abs]);
    }

    public IEnumerable<(bool IsCurrent, T Data)> GetWindowItemsWithCurrent<T>(IEnumerable<T> items)
    {
        var cc = Offset;
        foreach(var item in items.Skip(Offset).Take(WindowSize))
        {
            yield return (cc == CurrentItemIndex, item);
            cc++;
        }
    }

}

