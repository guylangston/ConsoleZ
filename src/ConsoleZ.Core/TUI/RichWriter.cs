namespace ConsoleZ.Core.TUI;

public interface IRichWriter<TClr, TStyle> : ITextWriter
{
    TClr Fore { get; set; }
    TClr Back { get; set; }
    TStyle Style { get; set; }
    void Revert();
    void Push();
    void Pop();

    void Write(TClr fg, ReadOnlySpan<char> txt);
}

public abstract class RichWriter<TClr, TStyle> : TextWriterBase
{
    protected RichWriter(TClr fore, TClr back, TStyle style)
    {
        Fore = fore;
        Back = back;
        Style = style;
        StateStack.Push(Default = new RenderState(fore, back, style));
    }

    public record RenderState(TClr Fore, TClr Back, TStyle Style);

    public virtual Stack<RenderState> StateStack { get; } = new();
    public void Push() => StateStack.Push(State);
    public void Pop()  => State = StateStack.Pop();
    public virtual RenderState Default { get; set; }

    // Current colours, style to be applied to each Write...
    public virtual TClr Fore { get; set; }
    public virtual TClr Back { get; set; }
    public virtual TStyle Style { get; set; }
    public RenderState State
    {
        get => new RenderState(Fore, Back, Style);
        set
        {
            Fore = value.Fore;
            Back = value.Back;
            Style = value.Style;
        }
    }

    /// <summary>NOTE: Reverts after write</summary>
    public virtual void Write(TClr fg, ReadOnlySpan<char> txt)
    {
        Fore = fg;
        Write(txt);
        Revert();
    }

    public virtual void Write(TClr fg, TClr bg, ReadOnlySpan<char> txt)
    {
        Fore = fg;
        Back = bg;
        Write(txt);
        Revert();
    }

    public void Set(TClr fg) { Fore = fg; }
    public void Set(TClr fg, TClr bg) { Fore = fg; Back = bg; }
    public void Set(TClr fg, TClr bg, TStyle style) { Fore = fg; Back = bg; Style = style; }

    public virtual void Revert()
    {
        var curr = StateStack.Peek();
        State = curr;
    }

}

