namespace ConsoleZ.Core.Buffer;

public static class BufferHelper
{
    public static void Fill<T>(this IBuffer<T> buf, T cell)
    {
        for (int x = 0; x < buf.Width; x++)
            for (int y = 0; y < buf.Height; y++)
                buf[x, y] = cell;
    }

    public static void Write<T>(this IBuffer<T> buf, int x, int y, ReadOnlySpan<T> seq)
    {
        foreach (var c in seq)
        {
            buf[x++, y] = c;
            if (x >= buf.Width) return;
        }
    }

    public static void Write<TClr>(this IScreenBuffer<TClr> buf, int x, int y, TClr fg, TClr bg, ReadOnlySpan<char> text)
    {
        foreach (var c in text)
        {
            buf[x++, y] = new ScreenCell<TClr>(fg, bg, c);
            if (x >= buf.Width) return;
        }
    }
}

