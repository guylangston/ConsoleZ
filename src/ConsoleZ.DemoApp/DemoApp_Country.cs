namespace ConsoleZ.Core.DemoApp;

public class CountryListScene : DemoSceneBase
{
    ListView<ConsoleColor, Country>? view;
    IScreenBuffer<ConsoleColor>? popup;
    CommandSet<ConsoleKey> commands = new();

    public CountryListScene() : base(new MyStyle(StyleProviderTemplates.CreateStdConsole()))
    {
        RegisterCommands();
    }

    void ToggleHelp()
    {
        if (popup == null)
        {
            popup = new ScreenBuffer(50, 5);
            var fg = Style.GetColourOrDefault("Popup.Fg");
            var bg = Style.GetColourOrDefault("Popup.Bg", false);
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
            var fg = Style.GetColourOrDefault("Popup.Fg");
            var bg = Style.GetColourOrDefault("Popup.Bg", false);
            popup.DrawBox(fg, bg,  Glyphs.Double);
            popup.Write(3, 2, fg, bg, "Hello World");
        }
        else
        {
            popup = null;
        }
    }

    void RegisterCommands()
    {
        var quit = commands.Register(CommandFactory.Create("Quit", ()=>Host.RequestQuit()));
        commands.Map(ConsoleKey.Q, quit);
        commands.Map(ConsoleKey.Escape, quit);
        commands.Map(ConsoleKey.LeftArrow,  CommandFactory.Create("MoveLeft",    ()=>view?.MoveLeft()));
        commands.Map(ConsoleKey.RightArrow, CommandFactory.Create("MoveRight",   ()=>view?.MoveRight()));
        commands.Map(ConsoleKey.UpArrow,    CommandFactory.Create("MoveUp",      ()=>view?.MoveUp()));
        commands.Map(ConsoleKey.DownArrow,  CommandFactory.Create("MoveDown",    ()=>view?.MoveDown()));
        commands.Map(ConsoleKey.Home,       CommandFactory.Create("MoveFirst",   ()=>view?.First()));
        commands.Map(ConsoleKey.End,        CommandFactory.Create("MoveLast",    ()=>view?.Last()));
        commands.Map(ConsoleKey.F1,         CommandFactory.Create("ToggleHelp",  ToggleHelp));
        commands.Map(ConsoleKey.P,          CommandFactory.Create("TogglePopup", TogglePopup));
    }

    class MyStyle : DemoSceneBase.StyleProvider
    {
        public readonly TextClr<ConsoleColor> Popup;

        public MyStyle(StyleProviderStd<ConsoleColor> copy)
            : base(copy.Palette, new Dictionary<string, ConsoleColor>(copy.StyleToColour), copy.DefaultFore, copy.DefaultBack)
        {
            Popup = Set(nameof(Popup), ConsoleColor.Yellow, ConsoleColor.DarkBlue);
        }
    }


    public override void Draw(ScreenBuffer buffer)
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

        if (view == null)
        {
            var cols = listArea.Width >= 80 ? 3 : 2;
            view = new ListView<ConsoleColor, Country>(SampleCountry.Countries, new LayoutGrid<ConsoleColor>(listArea, cols, 4));
            // view = new ListView<ConsoleColor, Country>(SampleCountry.Countries, new LayoutStack<ConsoleColor>(listArea, Orientation.Vert, 4));
        }

        foreach(var segment in view.GetViewData())
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

    protected override bool TryHandleKey(HandleKey type, ConsoleKeyInfo key)
    {
        if (view == null || commands == null) return false;

        var res = false;
        foreach(var map in commands.Mappings)
        {
            if (map.Input == key.Key)
            {
                try
                {
                    error = null;
                    map.Command.Execute(new CommandContext(Host, App, this), CommandArgs.Empty);
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
