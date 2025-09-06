namespace ConsoleZ.Core.DemoApp;

public class CountryListScene : DemoSceneBase
{
    IScreenBuffer<ConsoleColor>? popup;

    // NOTE: We need a readonly reference to attach the `CommandSet` to. This means state, data, buffer may vary
    readonly ListView<ConsoleColor, Country> viewMain = new(new ListViewState());

    public CountryListScene() : base(new MyStyle(StyleProviderTemplates.CreateStdConsole()))
    {
    }

    void ToggleHelp()
    {
        if (popup == null)
        {
            popup = new ScreenBuffer(50, 5);
            var fg = Style.GetFg("Popup.Fg");
            var bg = Style.GetBg("Popup.Bg");
            popup.DrawBox(fg, bg,  Glyphs.Double);

            var writer = popup.Inset(2, 1).CreateWriter(fg, bg);
            writer.WriteLine("--- HELP ---");
            writer.WriteLine(" (p) Toggle Popup");
            writer.WriteLine(" (F1) Show help");
        }
        else
        {
            popup = null;
        }
    }

    void TogglePopup()
    {
        if (popup == null)
        {
            popup = new ScreenBuffer(50, 5);
            var fg = Style.GetFg("Popup.Fg");
            var bg = Style.GetBg("Popup.Bg");
            popup.DrawBox(fg, bg,  Glyphs.Double);
            popup.Write(3, 2, fg, bg, "Hello World");
        }
        else
        {
            popup = null;
        }
    }

    protected override void InitCommands(CommandSet<ConsoleKey> commands)
    {
        var quit = commands.Register(CommandFactory.Create("Quit", ()=>Host.RequestQuit()));
        commands.Map(ConsoleKey.Q, quit);
        commands.Map(ConsoleKey.Escape, quit);
        commands.Map(ConsoleKey.UpArrow,    CommandFactory.Create("Up",           () => viewMain.MoveUp()));
        commands.Map(ConsoleKey.RightArrow, CommandFactory.Create("Right",        () => viewMain.MoveRight()));
        commands.Map(ConsoleKey.LeftArrow,  CommandFactory.Create("Left",         () => viewMain.MoveLeft()));
        commands.Map(ConsoleKey.DownArrow,  CommandFactory.Create("Down",         () => viewMain.MoveDown()));
        commands.Map(ConsoleKey.F1,         CommandFactory.Create("ToggleHelp",   ToggleHelp));
        commands.Map(ConsoleKey.P,          CommandFactory.Create("TogglePopup",  TogglePopup));
        commands.Map(ConsoleKey.H,          CommandFactory.Create("ToggleHeader", ()=> { IsHeaderEnabled = !IsHeaderEnabled; }));
        commands.Map(ConsoleKey.F,          CommandFactory.Create("ToggleFooter", ()=> { IsFooterEnabled = !IsFooterEnabled; }));

    }

    public class MyStyle : StyleProviderBase
    {
        public readonly TextClr<ConsoleColor> Popup;

        public MyStyle(StyleProviderStd<ConsoleColor> copy)
            : base(copy.Palette, new Dictionary<string, ConsoleColor>(copy.StyleToColour), copy.DefaultFore, copy.DefaultBack)
        {
            Popup = Set(nameof(Popup), ConsoleColor.Yellow, ConsoleColor.DarkBlue);
        }
    }

    public override void Draw(IScreenBuffer<ConsoleColor> buffer)
    {
        base.Draw(buffer);
        if (popup != null)
        {
            buffer.DrawBuffer(popup, buffer.Width/2 - popup.Width/2, buffer.Height/2 - popup.Height/2);
        }
    }

    protected override void DrawBody(IScreenBuffer<ConsoleColor> body)
    {
        var (listArea, detailArea) = body.SplitVert(60);

        var cols = listArea.Width >= 80 ? 3 : 2;

        // NOTE: Data and Layout/Buffer may change between each from by some external agent... 
        //       To be safe, verify and reset them as needed
        viewMain.VerifySource(SampleCountry.Countries);
        viewMain.VerifyLayout(new LayoutGrid<ConsoleColor>(listArea, cols, body.Height/2));

        foreach(var segment in viewMain.GetViewData())
        {
            var inner = segment.Segment.Buffer;
            var data = segment.Data;

            var (fg, bg) = segment.IsCursor ? Style.Selected : Style.Body;
            var writer = new RichWriterScreenBuffer<ConsoleColor, string>(inner, fg, bg, "");
            inner.Fill(fg, bg, ' ');
            writer.Write($"#{segment.DataIdx} ");
            writer.Write(Style.Highlight, data.Code);
            writer.Write(' ');
            writer.Write(data.Name);
            writer.WriteLine();
            writer.Write("   ");
            writer.Write(data.Capital);

            if (segment.IsCursor)
            {
                ScreenBufferHelper.DrawBox(detailArea, Style.Body.Fg, Style.Body.Bg, Glyphs.Single);
                var det = new RichWriterScreenBuffer<ConsoleColor, string>(detailArea.Inset(2, 1), Style.Body.Fg, Style.Body.Bg, "");
                WriteProp("Name", data.Name);
                WriteProp("Code", data.Code);
                WriteProp("Population", data.Population);
                WriteProp("Capital", data.Capital);
                WriteProp("Flag", data.UnicodeFlag);

                void WriteProp(string name, object val)
                {
                    det.Write(name.PadLeft(10));
                    det.Write(Style.Lowlight, ": ");
                    det.Write(Style.Highlight, val.ToString());
                    det.WriteLine();
                }
            }
        }
    }

    string? error = null;
    // protected override void DrawFooter(IScreenBuffer<ConsoleColor> footer)
    // {
    //     footer.Write(0,0, Style.Footer.Fg, Style.Footer.Bg, $"Time: {DateTime.Now} -- [Q]uit or <ESC>   {Host.Timer.FPS:0}fps --- {error}");
    // }

    protected override bool TryHandleKey(HandleKey type, ConsoleKey key)
    {
        if (Commands == null) return false;

        var res = false;
        foreach(var map in Commands.Mappings)
        {
            if (map.Input == key)
            {
                try
                {
                    error = null;
                    map.Command.Execute(new CommandContext(Host, App, this, null), CommandArgs.Empty);
                    res = true;
                    // dont exit, may be more than one mapping
                }
                catch(NotImplementedException)
                {
                    error = "NotImplemented";
                }
            }
        }

        return res;
    }

}
