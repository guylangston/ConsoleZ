namespace ConsoleZ.Core.Enriched;

// https://gist.github.com/ConnerWill/d4b6c776b509add763e17f9f113fd25b
// https://invisible-island.net/xterm/ctlseqs/ctlseqs.html#h2-Controls-beginning-with-ESC
// https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences
public static class TerminalString
{
    // Create a terminal encoded string setting the color, then reverting after appending txt
    public static string WrapEscape(string txt, string esc) => $"\u001b[{esc}{txt}\u001b[0m";

    public static string WrapInColour(string txt, ConsoleColor fg) => $"\u001b[{Get8Color(fg)}m{txt}\u001b[0m";
    public static string WrapInColour(string txt, ConsoleColor fg, ConsoleColor bg) => $"\u001b[{Get8Color(fg)};{Get8Color(bg, true)}m{txt}\u001b[0m";

    public static string SetColour(string txt, ConsoleColor fg) => $"\u001b[{Get8Color(fg)}m{txt}";
    public static string SetColour(string txt, ConsoleColor fg, ConsoleColor bg) => $"\u001b[{Get8Color(fg)};{Get8Color(bg, true)}m{txt}";

    public enum Color8 { Black, Red, Green, Yellow, Blue, Magenta, Cyan, White, Default, Reset }

    //  public enum ConsoleColor
    //  Black, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, Gray,
    //  DarkGray, Blue, Green, Cyan, Red, Magenta, Yellow, White
    private static readonly Color8[] MapConsoleColor =
        [
        Color8.Black,   Color8.Blue, Color8.Green, Color8.Cyan, Color8.Red, Color8.Magenta, Color8.Yellow, Color8.Default,
        Color8.Default, Color8.Blue, Color8.Green, Color8.Cyan, Color8.Red, Color8.Magenta, Color8.Yellow, Color8.Reset
        ];

    // https://gist.github.com/ConnerWill/d4b6c776b509add763e17f9f113fd25b#8-16-colors
    public static int Get8Color(Color8 clr, bool isBg = false, bool isBright = false)
    {
        if (clr == Color8.Reset) return 0;
        var n = (int)clr;
        if (isBright)
        {
            return isBg ? 100 + n : 90 + n;
        }
        return isBg ? 40 + n : 30 + n;
    }

    public const string SaveScreenState = "\u001b[s";
    public const string RestoreScreenState = "\u001b[u";


    public const string SaveCursorPosition = "\u001b7";
    public const string RestoreCursorPosition = "\u001b8";

    public static int Get8Color(ConsoleColor cons, bool isBg = false)
    {
        var n = (int)cons;
        var c8 = MapConsoleColor[n];
        return Get8Color(c8, isBg, n > 7);
    }
}
