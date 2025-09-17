using System.Diagnostics;
using ConsoleZ.Core.Buffer;
using ConsoleZ.Core.Enriched;

namespace ConsoleZ.Core.TUI;

public sealed class TextApplicationReservedLines<TInput> : ITextApplication, ITextApplicationInput<TInput>
{
    readonly ScreenBuffer buffer;
    readonly ITextScene<IScreenBuffer<ConsoleColor>, TInput> scene;
    ITextApplicationHost? host;

    public TextApplicationReservedLines(int reservedLines, ITextScene<IScreenBuffer<ConsoleColor>, TInput> scene)
    {
        ReservedLines = reservedLines;
        this.buffer = new ScreenBuffer(Console.WindowWidth, ReservedLines);
        this.scene = scene;
        this.InitialConsoleState = null;
    }

    public int ReservedLines { get; private init; }
    public int StartLine { get; private set; }
    public int EndLine { get; private set; }
    public int Width => buffer.Width;
    public int Height => ReservedLines;
    public bool ClearBeforeDraw { get; set; } = true;
    public bool CleanOnFinalDraw { get; set; } = false;
    public ConsoleState? InitialConsoleState { get; private set; }

    /// <summary>
    /// Complex unicode surrogate items are replaced with '?'.
    /// As some unicode chars take more then one cell, making buffers render incorrectly
    /// </summary>
    public bool IgnoreSurrogate { get; set; } = true;
    public ITextApplicationHost Host => host ?? throw new NullReferenceException("Only available after Init");

    public void Step()
    {
        scene.Step();
    }

    public void Init(ITextApplicationHost host)
    {
        this.host = host;
        this.InitialConsoleState = ConsoleState.Capture();
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
        if (Host is TextApplicationHost th)
        {
            if (th.HostDrawContext.IsPauseStart)
            {
                Debug.Assert(InitialConsoleState != null);
                Console.ForegroundColor = InitialConsoleState!.Fg;
                Console.BackgroundColor = InitialConsoleState.Bg;
                buffer.Fill(InitialConsoleState.Fg, InitialConsoleState.Bg);
                CopyToConsole(buffer);
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, StartLine);
                return;
            }
            if (th.HostDrawContext.IsPauseEnd)
            {
                Console.CursorVisible = false;
                buffer.Fill(ConsoleColor.DarkGray, ConsoleColor.Black, ' ');
            }
        }
        if (ClearBeforeDraw)
        {
            buffer.Fill(ConsoleColor.DarkGray, ConsoleColor.Black, ' ');
        }
        scene.Draw(buffer);
        if (CleanOnFinalDraw && Host is TextApplicationHost th2 && th2.HostDrawContext.IsFinalFrame)
        {
            buffer.Fill(ConsoleColor.DarkGray, ConsoleColor.Black, ' ');
            CopyToConsole(buffer);
            Console.CursorTop = StartLine;
            return;
        }
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
                    if (IgnoreSurrogate)
                    {
                        Console.Write('?');
                    }
                    else
                    {
                        Console.Write(cell.Chr);
                    }
                }
                else
                {
                    Console.Write(cell.Chr);
                }
            }
            Console.WriteLine();
        }
    }

    public bool HandleKey(HandleKey type, TInput key)
    {
        return scene.HandleKey(type, key);
    }
}

