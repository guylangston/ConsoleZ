using ConsoleZ.Core.Buffer;

namespace ConsoleZ.Core.TUI;

public sealed class TextApplicationReservedLines : ITextApplication, ITextApplicationInput<ConsoleKeyInfo>
{
    ScreenBuffer buffer;
    ITextApplicationHost? host;
    ITextScene<ScreenBuffer, ConsoleKeyInfo> scene;

    public TextApplicationReservedLines(int reservedLines, ITextScene<ScreenBuffer, ConsoleKeyInfo> scene)
    {
        ReservedLines = reservedLines;
        this.buffer = new ScreenBuffer(Console.WindowWidth, ReservedLines);
        this.scene = scene;
    }

    public int ReservedLines { get; private init; }
    public int StartLine { get; private set; }
    public int EndLine { get; private set; }
    public int Width => buffer.Width;
    public int Height => ReservedLines;
    public bool ClearBeforeDraw { get; set; } = true;

    public ITextApplicationHost Host => host ?? throw new NullReferenceException("Only available after Init");

    public void Step()
    {
        scene.Step();
    }

    public void Init(ITextApplicationHost host)
    {
        this.host = host;
        if (Console.WindowHeight <= ReservedLines) throw new Exception("ReservedLines larger than screensize");
        for(int cc=0; cc<ReservedLines; cc++)
        {
            Console.WriteLine();
        }
        EndLine = Console.CursorTop;
        StartLine = EndLine - ReservedLines;

        scene.Init(this, buffer.Width, buffer.Height);
    }

    public void Draw()
    {
        if (ClearBeforeDraw)
        {
            buffer.Fill(ConsoleColor.DarkGray, ConsoleColor.Black, ' ');
        }
        scene.Draw(buffer);
        CopyToConsole(buffer);
    }

    void CopyToConsole(ScreenBuffer buffer)
    {
        Console.SetCursorPosition(0, StartLine);
        ConsoleColor fg = Console.ForegroundColor;
        ConsoleColor bg = Console.BackgroundColor;

        for(var y=0; y<buffer.Height; y++)
        {
            for(var x=0; x<buffer.Width; x++)
            {
                var cell = buffer[x,y];
                if (fg != cell.Fg)
                {
                    Console.ForegroundColor = fg = cell.Fg;
                }
                if (bg != cell.Bg)
                {
                    Console.BackgroundColor = bg = cell.Bg;
                }
                if (char.IsSurrogate(cell.Chr))
                {
                    Console.Write('?');
                    // Console.Write($" {cell.Chr}");
                    // Not sure how to handle this?
                }
                else
                {
                    Console.Write(cell.Chr);
                }
            }
            Console.WriteLine();
        }
    }

    public void HandleKey(HandleKey type, ConsoleKeyInfo key)
    {
        scene.HandleKey(type, key);
    }
}

