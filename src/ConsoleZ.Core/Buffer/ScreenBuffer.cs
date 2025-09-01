namespace ConsoleZ.Core.Buffer;

public class ScreenBuffer<TClr> : ArrayBuffer<ScreenCell<TClr>>, IScreenBuffer<TClr>
{
    public ScreenBuffer(ScreenCell<TClr>[,] inner) : base(inner) { }

    public ScreenBuffer(int width, int height) : base(width, height) { }

    public void Set(int x, int y, TClr fg, TClr bg, char chr) => this[x, y] = new ScreenCell<TClr>(fg, bg, chr);

    public void Fill(TClr fg, TClr bg, char chr) => BufferHelper.Fill(this, new ScreenCell<TClr> { Chr = chr, Fg = fg, Bg = bg });

    public static ScreenBuffer<TClr> FromText(TClr fg, TClr bg, ReadOnlySpan<char> txt)
    {
        var buf = new ScreenBuffer<TClr>(txt.Length, 1);
        buf.Write(0, 0, fg, bg, txt);
        return buf;
    }


}

public class ScreenBuffer : ScreenBuffer<ConsoleColor>
{
    public ScreenBuffer(ScreenCell<ConsoleColor>[,] inner) : base(inner) { }

    public ScreenBuffer(int width, int height) : base(width, height) { }

}
