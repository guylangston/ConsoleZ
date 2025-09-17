namespace ConsoleZ.Core.Models;

public class ScrollableListModel
{
    public int ListSize { get; set; }
    public int WindowSize { get; set; }
    public int Offset { get; protected set; }

    public ScrollableListModel( int windowSize, int listSize)
    {
        ListSize = listSize;
        WindowSize = windowSize;
        Offset = 0;
    }

    public virtual bool Previous()
    {
        if (Offset <= 0) return false;
        Offset--;
        return true;
    }

    public virtual bool Next()
    {
        if (Offset + WindowSize >= ListSize) return false;
        Offset++;
        return true;
    }

    public virtual void First() => Offset = 0;
    public virtual void Last()
    {
        if (WindowSize >= ListSize)
        {
            Offset = 0;
        }
        else
        {
            Offset = ListSize - WindowSize;
        }
    }

    public IEnumerable<T> GetWindowItems<T>(IEnumerable<T> items)
    {
        return  items.Skip(Offset).Take(WindowSize);
    }
}
