namespace ConsoleZ.Core;

public static class Glyphs
{
    public record BoxChar(
            char TopLeft, char TopMiddle, char TopRight,
            char MiddleLeft, char Middle, char MiddleRight,
            char BottomLeft, char BottomMiddle, char BottomRight);

    public static readonly BoxChar SingleText = new BoxChar(
            '/', '-', '\\',
            '|', ' ', '|',
            '\\', '-', '/' );

    public static readonly BoxChar Single = new BoxChar(
            '┌', '─', '┐',
            '│', ' ', '│',
            '└', '─', '┘' );

    public static readonly BoxChar Double = new BoxChar(
            '╔', '═', '╗',
            '║', ' ', '║',
            '╚', '═', '╝' );

    public static char Tick        = '✔';
    public static char Cross       = '✖';
    public static char Warning     = '⚠';
    public static char Info        = 'ℹ';
    public static char Question    = '❓';
    public static char Star        = '★';
    public static char ArrowRight  = '→';
    public static char ArrowLeft   = '←';
    public static char ArrowUp     = '↑';
    public static char ArrowDown   = '↓';
    public static char CircleEmpty = '○';
    public static char CircleFull  = '●';
}

