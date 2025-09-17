

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

    public const char Tick           = '✔';
    public const char Cross          = '✖';
    public const char Warning        = '⚠';
    public const char Info           = 'ℹ';
    public const char Question       = '⍰';
    public const char Star           = '★';
    public const char ArrowRight     = '→';
    public const char ArrowLeft      = '←';
    public const char ArrowUp        = '↑';
    public const char ArrowDown      = '↓';
    public const char ArrowDownRight = '↳';

    public const char CircleEmpty = '○';
    public const char CircleFull  = '●';

    // single-char unicode only
    public const string Folder  = "🗀";
    public const string File  = "🗎";
}

