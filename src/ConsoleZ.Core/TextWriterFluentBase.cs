namespace ConsoleZ.Core;

public abstract class TextWriterFluentBase<TSelf> : ITextWriterFluent<TSelf> where TSelf : TextWriterFluentBase<TSelf>
{
    ITextWriter inner;

    protected TextWriterFluentBase(ITextWriter inner)
    {
        this.inner = inner;
    }

    protected TSelf Self => (TSelf)this;

    public TSelf Write(char chr)
    {
        inner.Write(chr);
        return Self;
    }

    public TSelf Write(string s)
    {
        inner.Write(s);
        return Self;
    }

    public TSelf Write(ReadOnlySpan<char> s)
    {
        inner.Write(s);
        return Self;
    }

    public TSelf Write<T>(T? obj)
    {
        inner.Write(obj);
        return Self;
    }

    public TSelf WriteLine()
    {
        inner.WriteLine();
        return Self;
    }

    public TSelf WriteLine(ReadOnlySpan<char> s)
    {
        inner.WriteLine(s);
        return Self;
    }

    public TSelf WriteLine(string s)
    {
        inner.WriteLine(s);
        return Self;
    }

    public TSelf WriteLine<T>(T? obj)
    {
        inner.WriteLine(obj);
        return Self;
    }
}


