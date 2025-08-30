using ConsoleZ.Core.TUI;

namespace ConsoleZ.Core.Buffer;

public static class ScreenBufferHelper
{
    public static void Write<TClr>(this IScreenBuffer<TClr> buf, int x, int y, TClr fg, TClr bg, ReadOnlySpan<char> txt, bool wrapElseClip)
    {
        foreach (var c in txt)
        {
            if (c == '\n')
            {
                x = 0;
                y++;
                if (y >= buf.Height) return;
            }

            buf[x, y] = new ScreenCell<TClr> { Chr = c, Fg = fg, Bg = bg };
            x++;
            if (x >= buf.Width)
            {
                if (wrapElseClip)
                {
                    x = 0;
                    y++;
                }
                else
                {
                    return;
                }
            }
        }
    }

    public static void Draw<TClr>(this IScreenBuffer<TClr>  buf, IScreenBuffer<TClr> src, int px = 0, int py = 0)
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
        buf.Set(x++,y, fg, bg, l);
        for(var cc=0; cc < buf.Width - 3; cc++) buf.Set(x++,y, fg, bg, m);
        buf.Set(x,y++, fg, bg, r);

        l = box.MiddleLeft; m = box.Middle; r = box.MiddleRight;
        for(var ln=0; ln< buf.Height-2; ln++)
        {
            x = 0;
            buf.Set(x++,y, fg, bg, l);
            for(var cc=0; cc < buf.Width - 3; cc++) buf.Set(x++,y, fg, bg, m);
            buf.Set(x,y++, fg, bg, r);
        }

        l = box.BottomLeft; m = box.BottomMiddle; r = box.BottomRight;
        x = 0;
        buf.Set(x++,y, fg, bg, l);
        for(var cc=0; cc < buf.Width - 3; cc++) buf.Set(x++,y, fg, bg, m);
        buf.Set(x,y++, fg, bg, r);
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
}

