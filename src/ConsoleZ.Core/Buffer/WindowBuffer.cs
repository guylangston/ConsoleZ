namespace ConsoleZ.Core.Buffer;

public static class WindowBuffer
{
    public static WindowBuffer<T> FromBuffer<T>(IBuffer<T> buffer, int innerX, int innerY, int width, int height)
    {
        return new WindowBuffer<T>(buffer, innerX, innerY, width, height);
    }

    public static WindowScreenBuffer<TClr> FromBuffer<TClr>(IScreenBuffer<TClr> buffer, int innerX, int innerY, int width, int height)
    {
        return new WindowScreenBuffer<TClr>(buffer, innerX, innerY, width, height);
    }

    public static (WindowScreenBuffer<TClr> Left, WindowScreenBuffer<TClr> Right) SplitVert<TClr>(this IScreenBuffer<TClr> buffer)
    {
        return (
                new WindowScreenBuffer<TClr>(buffer, 0, 0, buffer.Width/2, buffer.Height),
                new WindowScreenBuffer<TClr>(buffer, 0 + buffer.Width/2, 0, buffer.Width/2,buffer.Height)
               );
    }

    public static WindowScreenBuffer<TClr> Inset<TClr>(this IScreenBuffer<TClr> buffer, int size) => Inset(buffer, size, size);

    public static WindowScreenBuffer<TClr> Inset<TClr>(this IScreenBuffer<TClr> buffer, int sizeX, int sizeY)
    {
        return new WindowScreenBuffer<TClr>(buffer, sizeX, sizeY, buffer.Width - sizeX *2, buffer.Height - sizeY*2);
    }
}

public struct WindowBuffer<T> : IBuffer<T>
{
    IBuffer<T> inner;
    int innerX;
    int innerY;
    int width;
    int height;

    public WindowBuffer(IBuffer<T> inner, int innerX, int innerY, int width, int height)
    {
        this.inner = inner;
        if (innerX >= inner.Width) throw new InvalidDataException("Not within inner buffer");
        if (innerY >= inner.Height) throw new InvalidDataException("Not within inner buffer");
        if (innerX + width > inner.Width) throw new InvalidDataException("Not within inner buffer");
        if (innerY + height > inner.Height) throw new InvalidDataException("Not within inner buffer");
        this.innerX = innerX;
        this.innerY = innerY;
        this.width = width;
        this.height = height;
    }

    public T this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= width) throw new IndexOutOfRangeException($"X:{x} must be in [0..{width-1}]");
            if (y < 0 || y >= width) throw new IndexOutOfRangeException($"Y:{x} must be in [0..{width-1}]");
            return inner[innerX + x, innerY + y];
        }

        set
        {
            if (x < 0 || x >= width) throw new IndexOutOfRangeException($"X:{x} must be in [0..{width-1}]");
            if (y < 0 || y >= width) throw new IndexOutOfRangeException($"Y:{x} must be in [0..{width-1}]");
            inner[innerX + x, innerY + y] = value;
        }
    }

    public int Width => width;
    public int Height => height;


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

public struct WindowScreenBuffer<TClr> : IScreenBuffer<TClr>
{
    IScreenBuffer<TClr> inner;
    int innerX;
    int innerY;
    int width;
    int height;

    public WindowScreenBuffer(IScreenBuffer<TClr> inner, int innerX, int innerY, int width, int height)
    {
        this.inner = inner;
        if (innerX >= inner.Width) throw new InvalidDataException("Not within inner buffer");
        if (innerY >= inner.Height) throw new InvalidDataException("Not within inner buffer");
        if (innerX + width > inner.Width) throw new InvalidDataException("Not within inner buffer");
        if (innerY + height > inner.Height) throw new InvalidDataException("Not within inner buffer");
        this.innerX = innerX;
        this.innerY = innerY;
        this.width = width;
        this.height = height;
    }

    public ScreenCell<TClr> this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= width) throw new IndexOutOfRangeException($"X:{x} must be in [0..{width-1}]");
            if (y < 0 || y >= width) throw new IndexOutOfRangeException($"Y:{x} must be in [0..{width-1}]");
            return inner[innerX + x, innerY + y];
        }

        set
        {
            if (x < 0 || x >= width) throw new IndexOutOfRangeException($"X:{x} must be in [0..{width-1}]");
            if (y < 0 || y >= width) throw new IndexOutOfRangeException($"Y:{x} must be in [0..{width-1}]");
            inner[innerX + x, innerY + y] = value;
        }
    }

    public int Width => width;
    public int Height => height;

    public bool Contains(int x, int y)
    {
        if (x < 0 || x >= Width) return false;
        if (y < 0 || y >= Height) return false;
        return true;
    }

    public void SetClipped(int x, int y, ScreenCell<TClr> cell)
    {
        if (Contains(x, y))
        {
            this[x, y] = cell;
        }
    }

    public void Fill(TClr fg, TClr bg, char chr)
    {
        BufferHelper.Fill(this, new ScreenCell<TClr>(fg, bg, chr));
    }
}
