using ConsoleZ.Core.Buffer;
using ConsoleZ.Core.TUI;

public abstract class DemoSceneBase : MasterSceneApp<ConsoleColor, ConsoleKey>
{
    ConsoleKey? unhandledKey;
    ConsoleKey? lastKey;

    protected DemoSceneBase(StyleProviderBase style) : base(style)
    {
    }

    protected new StyleProviderBase Style { get => (StyleProviderBase)base.Style; }

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
            headerSegKeys.WriteFg(6, 0, Style.Highlight, lastKey.Value.ToString());
        }
        if (unhandledKey != null)
        {
            headerSegKeys.Fill(Style.Error);
            headerSegKeys.WriteTextOnly(0, 0, " Unhandled:");
            headerSegKeys.WriteTextOnly(12, 0,unhandledKey.Value.ToString());
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

    protected abstract bool TryHandleKey(HandleKey type, ConsoleKey key);

    public override bool HandleKey(HandleKey type, ConsoleKey key)
    {
        if (TryHandleKey(type, key))
        {
            unhandledKey = null;
            lastKey = key;
            return true;
        }
        else
        {
            lastKey = null;
            unhandledKey = key;
            return false;
        }
    }

    public class StyleProviderBase : StyleProviderStd<ConsoleColor>
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
        public readonly ConsoleColor AltBg;

        protected StyleProviderBase(IReadOnlyList<ConsoleColor> palette, Dictionary<string, ConsoleColor> colours, ConsoleColor defaultFore, ConsoleColor defaultBack) : base(palette, colours, defaultFore, defaultBack)
        {
            Body          = Set(nameof(Body),          DefaultFore,           DefaultBack);
            AltBg         = Set(nameof(AltBg),         ConsoleColor.DarkGray);

            Header        = Set(nameof(Header),        ConsoleColor.Black,    AltBg);
            HeaderSegment = Set(nameof(HeaderSegment), ConsoleColor.Gray,     AltBg);
            HeaderHilight = Set(nameof(HeaderHilight), ConsoleColor.Green);
            Footer        = Set(nameof(Footer),        ConsoleColor.DarkGreen, Header.Bg);
            Selected      = Set(nameof(Selected),      ConsoleColor.Black,     ConsoleColor.DarkBlue);
            Error         = Set(nameof(Error),         ConsoleColor.Yellow,   ConsoleColor.DarkRed);
            Highlight     = Set(nameof(Highlight),     ConsoleColor.Magenta);
            Lowlight      = Set(nameof(Lowlight),      ConsoleColor.DarkBlue);
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

