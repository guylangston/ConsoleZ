namespace ConsoleZ.Core.Enriched;

public record ConsoleState(int CursorLeft,int CursorTop,ConsoleColor Fg,ConsoleColor Bg)
{
    public static ConsoleState Capture()
    {
        return new (Console.CursorLeft,Console.CursorTop, Console.ForegroundColor,Console.BackgroundColor);
    }
}
