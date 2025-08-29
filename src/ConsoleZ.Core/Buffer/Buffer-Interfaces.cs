namespace ConsoleZ.Core.Buffer;

public interface IBuffer<T>
{
    int Width { get; }
    int Height { get; }
    T this[int x, int y] { get; set; }

    bool Contains(int x, int y);
    void SetClipped(int x, int y, T cell);
}

public struct ScreenCell<TClr>
{
    public ScreenCell(TClr fg, TClr bg, char chr)
    {
        Chr = chr;
        Fg = fg;
        Bg = bg;
    }

    public ScreenCell(ScreenCell<TClr> other)
    {
        Chr = other.Chr;
        Fg = other.Fg;
        Bg = other.Bg;
    }

    public char Chr { get; set; }
    public TClr Fg  { get; set; }
    public TClr Bg  { get; set; }
}

public interface IScreenBuffer<TClr> : IBuffer<ScreenCell<TClr>>
{
    void Set(int x, int y, TClr fg, TClr bg, char chr) => this[x, y] = new ScreenCell<TClr>(fg, bg, chr);
    void Fill(TClr fg, TClr bg, char chr);
}
