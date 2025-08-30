namespace ConsoleZ.Core.TUI;

public class RichWriterConsole : RichWriter<ConsoleColor, string>
{
    public RichWriterConsole(string style) : base(Console.ForegroundColor, Console.BackgroundColor, style)
    {
    }

    public override ConsoleColor Fore { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
    public override ConsoleColor Back { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }

    public override void Write(char chr) => Console.Write(chr);
    public override void Write(ReadOnlySpan<char> s) => Console.Write(new String(s));
    public override void WriteLine() => Console.WriteLine();
}

public class RichWriterFluentConsole : RichWriterFluent<ConsoleColor, string, RichWriterFluentConsole>
{
    public RichWriterFluentConsole(RichWriter<ConsoleColor, string> inner) : base(inner)
    {
    }
}

