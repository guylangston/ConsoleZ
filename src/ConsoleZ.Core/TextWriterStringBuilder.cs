namespace ConsoleZ.Core;

public class TextWriterStringBuilder : TextWriterBase
{
    private readonly StringBuilder sb;

    public TextWriterStringBuilder() : this(new StringBuilder()) { }
    public TextWriterStringBuilder(StringBuilder sb) { this.sb = sb; }

    public StringBuilder StringBuilder => sb;
    public override string? ToString() => sb.ToString();
    public override void Write(char chr) => sb.Append(chr);
    public override void Write(ReadOnlySpan<char> s) => sb.Append(s);
    public override void WriteLine() => sb.AppendLine();
}
