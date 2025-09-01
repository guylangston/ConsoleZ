using ConsoleZ.Core.Buffer;
using ConsoleZ.Core.TUI;

public abstract class DemoSceneBase : MasterSceneApp<ConsoleColor, ConsoleKeyInfo>
{
    ConsoleKeyInfo? unhandledKey;
    ConsoleKeyInfo? lastKey;

    protected DemoSceneBase(StyleProvider style) : base(style)
    {
    }

    protected new StyleProvider Style { get => (StyleProvider)base.Style; }

    public override void Init(ITextApplication app, int width, int height)
    {
        base.Init(app, width, height);
        headerSegments.Clear();

        headerSegments.Add(Style.HeaderSegment.CreateBuffer($" [[ {GetType().Name} ]] "));

        this.headerSegKeys = new ScreenBuffer(20, 1);
        headerSegKeys.Fill(Style.HeaderSegment.Fg, Style.HeaderSegment.Bg, ' ');
        headerSegments.Add(headerSegKeys);

        footerSegments.Clear();

        var txt = " [F1] Help, [Q] or [ESC] quit ";
        footerSegments.Add(Style.HeaderSegment.CreateBuffer(txt));

        this.footerTimer =  Style.HeaderSegment.CreateBuffer(45, 1);
        footerSegments.Add(footerTimer);
    }

    protected override void DrawHeader(IScreenBuffer<ConsoleColor> header)
    {
        var px = 1;
        foreach(var seg in headerSegments)
        {
            header.DrawBuffer(seg, px, 0);
            px += seg.Width;
            px +=2;
        }
    }
    protected override void DrawFooter(IScreenBuffer<ConsoleColor> footer)
    {
        var px = 1;
        foreach(var seg in footerSegments)
        {
            footer.DrawBuffer(seg, px, 0);
            px += seg.Width;
            px +=2;
        }
    }

    public override void Step()
    {
        headerSegKeys.Fill(Style.HeaderSegment.Fg, Style.HeaderSegment.Bg, ' ');
        if (lastKey != null)
        {
            headerSegKeys.WriteTextOnly(0, 0, " Key:");
            headerSegKeys.WriteFg(6, 0, Style.Highlight, lastKey.Value.Key.ToString());
        }
        if (unhandledKey != null)
        {
            headerSegKeys.Fill(Style.Error);
            headerSegKeys.WriteTextOnly(0, 0, " Unhandled:");
            headerSegKeys.WriteTextOnly(12, 0,unhandledKey.Value.Key.ToString());
        }

        var fwriter = new RichWriterScreenBuffer<ConsoleColor, string>(footerTimer, Style.Footer.Fg,  Style.HeaderSegment.Bg, "");
        fwriter.Write(" Time: ");
        fwriter.Write(Style.HeaderSegment.Fg, DateTime.Now.ToString());
        fwriter.Write(" | Frames: ");
        fwriter.Write(Style.HeaderSegment.Fg, Host.Timer.FPS.ToString("0"));
        fwriter.Write("fps");
    }

    protected List<IScreenBuffer<ConsoleColor>> headerSegments = new();
    protected List<IScreenBuffer<ConsoleColor>> footerSegments = new();
    private ScreenBuffer headerSegKeys;
    private ScreenBuffer<ConsoleColor> footerTimer;

    protected abstract bool TryHandleKey(HandleKey type, ConsoleKeyInfo key);

    public override void HandleKey(HandleKey type, ConsoleKeyInfo key)
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
        public readonly ConsoleColor HeaderHilight;

        public readonly TextClr<ConsoleColor> Footer;
        public readonly TextClr<ConsoleColor> Body;
        public readonly TextClr<ConsoleColor> Selected;
        public readonly TextClr<ConsoleColor> Error;
        public readonly ConsoleColor Highlight;
        public readonly ConsoleColor Lowlight;

        protected StyleProvider(IReadOnlyList<ConsoleColor> palette, Dictionary<string, ConsoleColor> colours, ConsoleColor defaultFore, ConsoleColor defaultBack) : base(palette, colours, defaultFore, defaultBack)
        {
            this.Body          = Set(nameof(Body),          DefaultFore,           DefaultBack);
            this.Header        = Set(nameof(Header),        ConsoleColor.Black,    ConsoleColor.DarkGray);
            this.HeaderSegment = Set(nameof(HeaderSegment), ConsoleColor.DarkBlue, ConsoleColor.DarkCyan);
            this.HeaderHilight = Set(nameof(HeaderHilight), ConsoleColor.Green);
            this.Footer        = Set(nameof(Footer),        ConsoleColor.Black,    ConsoleColor.DarkGray);
            this.Selected      = Set(nameof(Selected),      ConsoleColor.Cyan,     ConsoleColor.DarkBlue);
            this.Error         = Set(nameof(Error),         ConsoleColor.Yellow,   ConsoleColor.DarkRed);
            this.Highlight     = Set(nameof(Highlight),     ConsoleColor.Magenta);
            this.Lowlight      = Set(nameof(Lowlight),      ConsoleColor.DarkBlue);
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

