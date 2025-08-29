namespace ConsoleZ.Core.TUI;

public abstract class RichWriterFluent<TClr, TStyle, TSelf> : TextWriterFluentBase<TSelf>
    where TSelf: RichWriterFluent<TClr, TStyle, TSelf>
{
    RichWriter<TClr, TStyle> inner;

    protected RichWriterFluent(RichWriter<TClr, TStyle> inner) : base(inner)
    {
        this.inner = inner;
    }

    public RichWriter<TClr, TStyle> Inner => inner;

    public TSelf Push()
    {
        inner.Push();
        return Self;
    }
    public TSelf Pop()
    {
        inner.Pop();
        return Self;
    }
    public TSelf Revert()
    {
        inner.Revert();
        return Self;
    }
    public TSelf Set(TClr fg)                        { inner.Set(fg); return Self; }
    public TSelf Set(TClr fg, TClr bg)               { inner.Set(fg, bg); return Self; }
    public TSelf Set(TClr fg, TClr bg, TStyle style) { inner.Set(fg, bg, style); return Self; }
    public TSelf SetBack(TClr bg)                    { inner.Back = bg; return Self; }
    public TSelf SetStyle(TStyle style)                    { inner.Style = style; return Self; }

    public TSelf Write(TClr fg, ReadOnlySpan<char> txt)                        { inner.Set(fg); inner.Write(txt); return Self; }
}

