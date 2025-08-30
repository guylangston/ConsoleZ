namespace ConsoleZ.Core.DemoApp;

using ConsoleZ.Core.Models;

    public record Command(string Name, string Description, string? Help, Action Action);
    public record Mapping(ConsoleKey key, Command cmd);

    public class CommandSet
    {
        public List<Command> Commands { get; } = new();
        public List<Mapping> Mappings { get; } = new();

        public Command Register(Command cmd)
        {
            Commands.Add(cmd);
            return cmd;
        }

        public Command Register(string name, Action action, string? desc = null)
        {
            return Register(new Command(name, desc ?? name, null, action));
        }
        public Mapping RegisterAndMap(ConsoleKey key, Command cmd)
        {
            Register(cmd);

            var map = new Mapping(key, cmd);
            Mappings.Add(map);
            return map;
        }

        public Mapping RegisterAndMap(ConsoleKey key, string name, Action action, string? desc = null)
        {
            return RegisterAndMap(key, new Command(name, desc ?? name, null, action));
        }
    }

public class CountryListScene : DemoSceneBase
{
    List<Country> Items = new List<Country>(SampleCountry.Countries);
    ScrollableListWithCurrentItemModel? model = null;
    CommandSet? commands = null;
    IScreenBuffer<ConsoleColor>? popup;
    int cols = 4;

    public CountryListScene() : base(new MyStyle(StyleProviderTemplates.CreateStdConsole()))
    {
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

    protected override void DrawHeader(IScreenBuffer<ConsoleColor> header)
    {
        // header.Write(0,0, Style.Header.Fg, Style.Header.Bg,
        //         $"[[ {GetType().Name} ]] Items: {model?.CurrentItemIndex}/{Items.Count}, LastKey: TODO");
    }

    public override void Draw(ScreenBuffer buffer)
    {
        base.Draw(buffer);
        if (popup != null)
        {
            buffer.Draw(popup, buffer.Width/2 - popup.Width/2, buffer.Height/2 - popup.Height/2);
        }
    }

    protected override void DrawBody(IScreenBuffer<ConsoleColor> body)
    {
        var (listArea, detailArea) = body.SplitVert();

        var rowsPerCol = 2;
        var colSize = listArea.Width / cols;
        if (model == null)
        {
            // TODO: Convert to GridListModel (x,y) table grid
            model = new ScrollableListWithCurrentItemModel(body.Height / rowsPerCol * cols, Items.Count );

            commands = new CommandSet();

            var quit = commands.Register( "Quit", ()=>Host.RequestQuit());
            commands.RegisterAndMap(ConsoleKey.Q, quit);
            commands.RegisterAndMap(ConsoleKey.Escape, quit);

            commands.RegisterAndMap(ConsoleKey.LeftArrow , "MovePrev" , ()=>model.Previous());
            commands.RegisterAndMap(ConsoleKey.RightArrow, "MoveNext" , ()=>model.Next());
            commands.RegisterAndMap(ConsoleKey.UpArrow   , "MoveUp"   , ()=>model.Previous());
            commands.RegisterAndMap(ConsoleKey.DownArrow , "MoveDown" , ()=>model.Previous());
            commands.RegisterAndMap(ConsoleKey.Home      , "MoveFirst", ()=>model.First());
            commands.RegisterAndMap(ConsoleKey.End       , "MoveLast" , ()=>model.Last());

            commands.RegisterAndMap(ConsoleKey.F1        ,  "ToggleHelp", ()=>
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
                    });
            commands.RegisterAndMap(ConsoleKey.P,           "TogglePopup", ()=>
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
                    });

        }

        foreach(var segment in Layout.PartitionIntoColsThenRows<ConsoleColor>(listArea, cols, rowsPerCol))
        {
            var segmentIdx = segment.idx;
            if (model.TryGetWindowItemIndex(segmentIdx, out var itemIndex))
            {
                var inner = segment.buf;
                var item = model.GetWindowItemsWithCurrent(Items, segmentIdx);
                var data = item.Data;

                var (fg, bg) = item.IsCurrent ? Style.Selected : Style.Body;
                var writer = new RichWriterScreenBuffer<ConsoleColor, string>(inner, fg, bg, "");
                inner.Fill(fg, bg, ' ');
                writer.Write($"#{itemIndex} ");
                writer.Write(Style.Highlight, data.Code);
                writer.Write(' ');
                writer.Write(data.Name);
                writer.WriteLine();
                writer.Write(data.Capital);

                if (item.IsCurrent)
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
    }

    protected override void DrawFooter(IScreenBuffer<ConsoleColor> footer)
    {
        // footer.Write(0,0, Style.Footer.Fg, Style.Footer.Bg, $"Time: {DateTime.Now} -- [Q]uit or <ESC>   {Host.Timer.FPS:0}fps");
    }

    protected override bool TryHandleKey(HandleKey type, ConsoleKeyInfo key)
    {
        if (model == null || commands == null) return false;

        var res = false;
        foreach(var map in commands.Mappings)
        {
            if (map.key == key.Key)
            {
                map.cmd.Action();
                res = true;
                // dont exit, may be more than one mapping
            }
        }

        return res;
    }

    public override void Step()
    {
    }
}
