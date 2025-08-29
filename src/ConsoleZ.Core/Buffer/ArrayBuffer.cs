namespace ConsoleZ.Core.Buffer;

// [GL] Q: Not sure what this really adds (this could also just be a struct)
//      A: This class adds nothing, but the buffer interface allows mappings to partial regions within the array -- which is useful
public class ArrayBuffer<T> : IBuffer<T>
{
    protected T[,] inner;

    public ArrayBuffer(T[,] inner) { this.inner = inner; }
    public ArrayBuffer(int width, int height) { this.inner = new T[width, height]; }

    public T this[int x, int y]
    {
        get => inner[x,y];
        set => inner[x,y] = value;
    }

    public int Width => inner.GetLength(0);
    public int Height => inner.GetLength(1);

    public bool Contains(int x, int y)
    {
        if (x < 0 || x >= Width) return false;
        if (y < 0 || y >= Height) return false;
        return true;
    }

    public void SetClipped(int x, int y, T cell)
    {
        if (Contains(x, y))
        {
            this[x, y] = cell;
        }
    }
}

