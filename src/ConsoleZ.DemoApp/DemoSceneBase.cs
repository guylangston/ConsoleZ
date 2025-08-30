namespace ConsoleZ.Core.DemoApp;

using ConsoleZ.Core.Buffer;
using ConsoleZ.Core.TUI;

public abstract class DemoSceneBase : ITextScene<ScreenBuffer, ConsoleKeyInfo>
{
    ITextApplication? app;
    ConsoleKeyInfo? unhandledKey;
    ConsoleKeyInfo? lastKey;
    protected StyleProvider Style { get; }

    protected DemoSceneBase(StyleProvider style)
    {
        Style = style;
    }

    public void Init(ITextApplication app, int width, int height)
    {
        this.app = app;
    }

    protected ITextApplication App => app ?? throw new NullReferenceException("Init should be called first");
    protected ITextApplicationHost Host => app?.Host ?? throw new NullReferenceException("Init should be called first");

    protected abstract void DrawHeader(IScreenBuffer<ConsoleColor> header);
    protected abstract void DrawBody(IScreenBuffer<ConsoleColor> body);
    protected abstract void DrawFooter(IScreenBuffer<ConsoleColor> footer);

    public abstract void Step();

    public virtual void Draw(ScreenBuffer canvas)
    {
        // Header
        var header = WindowBuffer.FromBuffer(canvas, 0, 0, canvas.Width, 1);
        header.Fill(Style.Header.Fg, Style.Header.Bg, ' ');

        var writer = new RichWriterScreenBuffer<ConsoleColor, string>(header, Style.Header.Fg,  Style.HeaderSegment.Bg, "");
        writer.Write("--<< ");
        writer.Write(Style.HeaderSegment.Fg, GetType().Name);
        writer.Write(" >>--");
        writer.Write(" Key:");
        if (lastKey != null)
        {
            writer.Write(Style.HeaderSegment.Fg, lastKey.Value.Key.ToString());
        }
        if (unhandledKey != null)
        {
            writer.Write(Style.HeaderSegment.Fg, ConsoleColor.Red, unhandledKey.Value.Key.ToString());
        }
        DrawHeader(header);

        // Body
        //
        var body = WindowBuffer.FromBuffer(canvas, 0, 1, canvas.Width, canvas.Height-2);
        body.Fill(Style.Body.Fg, Style.Body.Bg, ' ');
        DrawBody(body);

        // Footer
        var footer = WindowBuffer.FromBuffer(canvas, 0, canvas.Height-1, canvas.Width, 1);
        footer.Fill(Style.Footer.Fg, Style.Footer.Bg, ' ');
        footer.Write(0,0, Style.Footer.Fg, Style.Footer.Bg, $"Time: {DateTime.Now} -- [Q]uit or <ESC>   {Host.Timer.FPS:0}fps");
        var fwriter = new RichWriterScreenBuffer<ConsoleColor, string>(footer, Style.Footer.Fg,  Style.HeaderSegment.Bg, "");
        fwriter.Write(" ");
        fwriter.Write(Style.HeaderSegment.Fg, DateTime.Now.ToString());
        fwriter.Write(" | ");
        fwriter.Write(Style.HeaderSegment.Fg, Host.Timer.FPS.ToString("0"));
        fwriter.Write("fps");
        fwriter.Write(" | ");
        fwriter.Write(" <q> or <ESC> to exit. <F1> Help");
        DrawFooter(footer);
    }

    protected abstract bool TryHandleKey(HandleKey type, ConsoleKeyInfo key);

    public void HandleKey(HandleKey type, ConsoleKeyInfo key)
    {
        if (TryHandleKey(type, key))
        {
            unhandledKey = null;
            lastKey = key;
        }
        else
        {
            lastKey = null;
            unhandledKey = key;
        }
    }

    protected class StyleProvider : StyleProviderStd<ConsoleColor>
    {
        public readonly TextClr<ConsoleColor> Header;
        public readonly TextClr<ConsoleColor> HeaderSegment;
        public readonly TextClr<ConsoleColor> Footer;
        public readonly TextClr<ConsoleColor> Body;
        public readonly TextClr<ConsoleColor> Selected;
        public readonly ConsoleColor Highlight;
        public readonly ConsoleColor Lowlight;

        protected StyleProvider(IReadOnlyList<ConsoleColor> palette, Dictionary<string, ConsoleColor> colours, ConsoleColor defaultFore, ConsoleColor defaultBack) : base(palette, colours, defaultFore, defaultBack)
        {
            this.Body          = Set(nameof(Body),          DefaultFore,         DefaultBack);
            this.Header        = Set(nameof(Header),        ConsoleColor.Black,  ConsoleColor.DarkGray);
            this.HeaderSegment = Set(nameof(HeaderSegment), ConsoleColor.Yellow, ConsoleColor.DarkGray);
            this.Footer        = Set(nameof(Footer),        ConsoleColor.Black,  ConsoleColor.DarkGray);
            this.Selected      = Set(nameof(Selected),      ConsoleColor.Cyan,   ConsoleColor.DarkBlue);
            this.Highlight     = Set(nameof(Highlight),     ConsoleColor.Magenta);
            this.Lowlight       = Set(nameof(Lowlight),     ConsoleColor.DarkBlue);
        }

        protected ConsoleColor Set(string style, ConsoleColor clr)
        {
            StyleToColour[style] = clr;
            return clr;
        }
        protected TextClr<ConsoleColor> Set(string style, ConsoleColor fg, ConsoleColor bg)
        {
            StyleToColour[$"{style}.Fg"] = fg;
            StyleToColour[$"{style}.Bg"] = bg;
            return new (fg, bg);
        }
    }
}

