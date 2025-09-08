using ConsoleZ.Core.TUI;

namespace ConsoleZ.Core.Buffer;

public static class ScreenBufferHelper
{

    /// <summary>
    /// Will clip, Wont throw on overflow
    /// Will respect newline char starting a `x`
    /// Allow negative x, y meaning rel top, rel bottom
    /// Nothing special for '\t'
    /// </summary>
    public static void WriteSeq<TClr>(this IScreenBuffer<TClr> buf,
            int x, int y, ReadOnlySpan<char> txt,
            Func<ScreenCell<TClr>, char, ScreenCell<TClr>> transform)
    {
        var xx = x >= 0 ? x : buf.Width + x;
        var yy = y >= 0 ? y : buf.Height + y;

        // start point on buffer
        if (!buf.Contains(xx, yy)) return;

        var px = xx;
        var py = yy;
        foreach(var c in txt)
        {
            if (c == '\n')
            {
                px = xx;
                py++;
            }
            else if (px >= buf.Width)
            {
                px++; // don't break as there be be a line break ahead
            }
            else
            {
                var before = buf[px, py];
                buf[px, py] = transform(before, c);
                px++;
            }
        }
    }

    public static void WriteTextOnly<TClr>(this IScreenBuffer<TClr> buf, int x, int y, ReadOnlySpan<char> txt)
        => WriteSeq(buf, x, y, txt, (before, chr) => new ScreenCell<TClr>(before.Fg, before.Bg, chr));

    public static void WriteFg<TClr>(this IScreenBuffer<TClr> buf, int x, int y, TClr fg, ReadOnlySpan<char> txt)
        => WriteSeq(buf, x, y, txt, (before, chr) => new ScreenCell<TClr>(fg, before.Bg, chr));

    public static void Write<TClr>(this IScreenBuffer<TClr> buf, int x, int y, TClr fg, TClr bg, ReadOnlySpan<char> txt)
        => WriteSeq(buf, x, y, txt, (before, chr) => new ScreenCell<TClr>(fg, bg, chr));

    public static void DrawBuffer<TClr>(this IScreenBuffer<TClr>  buf, IScreenBuffer<TClr> src, int px = 0, int py = 0)
    {
        for(int x=0;x<src.Width; x++)
        {
            for(int y=0; y<src.Height; y++)
            {
                var ppx = px + x;
                var ppy = py + y;
                buf.SetClipped(ppx, ppy, src[x,y]);
            }
        }
    }

    public static void DrawBox<TClr>(this IScreenBuffer<TClr> buf, TClr fg, TClr bg, Glyphs.BoxChar box)
    {
        int x = 0, y = 0;
        char l = box.TopLeft, m = box.TopMiddle, r = box.TopRight;
        DrawLine(l, m, r);

        l = box.MiddleLeft; m = box.Middle; r = box.MiddleRight;
        for(var ln=0; ln< buf.Height-2; ln++)
        {
            x = 0;
            DrawLine(l, m, r);
        }

        l = box.BottomLeft; m = box.BottomMiddle; r = box.BottomRight;
        x = 0;
        DrawLine(l, m, r);


        void DrawLine(char l, char m, char r)
        {
            buf.Set(x++,y, fg, bg, l);
            for(var cc=0; cc < buf.Width - 2; cc++) buf.Set(x++,y, fg, bg, m);
            buf.Set(x,y++, fg, bg, r);
        }
    }

    public static void FillLine<TClr>(this IScreenBuffer<TClr> buffer, int line, TClr fg, TClr bg, char chr)
    {
        for (int x = 0; x < buffer.Width; x++)
        {
            buffer[x, line] = new ScreenCell<TClr> { Chr = chr, Fg = fg, Bg = bg };
        }
    }

    public static RichWriterScreenBuffer<TClr, string> CreateWriter<TClr>(this IScreenBuffer<TClr> buffer, TClr fg, TClr bg, string style = "")
    {
        var fwriter = new RichWriterScreenBuffer<TClr, string>(buffer, fg, bg, style);
        return fwriter;
    }

    public static void UpdateCells<TClr>(this IScreenBuffer<TClr> buffer, Func<ScreenCell<TClr>, ScreenCell<TClr>> update)
    {
        for(var y=0; y<buffer.Height; y++)
            for(var x=0; x<buffer.Width; x++)
                buffer[x,y] = update(buffer[x,y]);
    }
}

