namespace ConsoleZ.Core.TUI;

public class RichWriterConsoleColor : RichWriter<ConsoleColor, string>
{
    public RichWriterConsoleColor(string style) : base(Console.ForegroundColor, Console.BackgroundColor, style)
    {
    }

    public override ConsoleColor Fore { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
    public override ConsoleColor Back { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }

    public override void Write(char chr) => Console.Write(chr);
    public override void Write(ReadOnlySpan<char> s) => Console.Write(new String(s));
    public override void WriteLine() => Console.WriteLine();
}

public class RichWriterFluentConsoleColor : RichWriterFluent<ConsoleColor, string, RichWriterFluentConsoleColor>
{
    public RichWriterFluentConsoleColor(RichWriter<ConsoleColor, string> inner) : base(inner)
    {
    }
}



