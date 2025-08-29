namespace ConsoleZ.Core;

public class TextWriterAdapterMinimal : ITextWriterMinimal
{
    private readonly TextWriter inner;

    public TextWriterAdapterMinimal(TextWriter inner)
    {
        this.inner = inner;
    }

    public void Write(ReadOnlySpan<char> s) => inner.Write(s);
    public void WriteLine() => inner.WriteLine();
}

public class TextWriterAdapter : TextWriterBase
{
    private readonly TextWriter inner;

    public TextWriterAdapter(TextWriter inner)
    {
        this.inner = inner;
    }

    public override void Write(char chr) => inner.Write(chr);
    public override void Write(ReadOnlySpan<char> s) => inner.Write(s);
    public override void WriteLine() => inner.WriteLine();
}

