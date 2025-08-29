using ConsoleZ.Core.Buffer;

namespace ConsoleZ.Core.TUI;

public class ScreenBufferRichWriter<TClr, TStyle> : RichWriter<TClr, TStyle>
{
    int x, y;
    IScreenBuffer<TClr> buffer;

    public ScreenBufferRichWriter(IScreenBuffer<TClr> buffer, TClr fore, TClr back, TStyle style) : base(fore, back, style)
    {
        this.buffer = buffer;
    }

    public int CursorX => x;
    public int CursorY => y;

    public override void Write(char chr)
    {
        if (x >= 0 && x < buffer.Width && 
            y >= 0 && y < buffer.Height)
            buffer[x++,y] = new ScreenCell<TClr>(Fore, Back, chr);
    }

    public override void Write(ReadOnlySpan<char> s)
    {
        foreach(var chr in s)
        {
            Write(chr);
        }
    }

    public override void WriteLine()
    {
        x = 0;
        y++;
    }
}


