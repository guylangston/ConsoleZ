namespace ConsoleZ.Core.Buffer;

public static class BufferHelper
{
    public static void Fill<T>(this IBuffer<T> buf, T cell)
    {
        for (int x = 0; x < buf.Width; x++)
            for (int y = 0; y < buf.Height; y++)
                buf[x, y] = cell;
    }

}

