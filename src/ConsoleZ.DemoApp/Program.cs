using ConsoleZ.Core.DemoApp;

internal class Program
{
    record Demo(string Name, string Desc, Action RunAction);

    private static int Main(string[] args)
    {
        Dictionary<string, Demo> demos = new();

        void AddDemo(string name, string desc, Action runner)
        {
            demos[name] = new Demo(name, desc, runner);
        }

        AddDemo("bounce", "Random coloured boxses bouncing around",  ()=>
        {
            var scene = new BouncingBoxScene();
            var app = new TextApplicationReservedLines(10, scene);
            var host = new TextApplicationHost(args, app, 10);
            host.Run();
        });

        AddDemo("app", "Sample Country List Browser app (Header, Body, Footer)", ()=>
        {
            var scene = new CountryListScene();
            var app = new TextApplicationReservedLines(10, scene);
            var host = new TextApplicationHost(args, app, 10);
            host.Run();
        });

        AddDemo("style", "list all styles", () =>
        {
           var style = StyleProviderTemplates.CreateStdConsole();
           var writer = new RichWriterConsole("");
           var cc = 0;
           foreach(var item in style.StyleToColour)
           {
               writer.Write($"{cc,3} - {item.Key,-12} ");
               writer.Write(item.Value, "Foreground");
               writer.Write(" ");
               writer.Write(ConsoleColor.Black, item.Value, " Background ");
               writer.Write(" ");
               writer.Write(ConsoleColor.White, item.Value, " Background ");
               writer.WriteLine();

               cc++;
           }
        });

        if (args.Length > 0 && demos.TryGetValue(args.First(), out var demo))
        {
            demo.RunAction();
            return 0;
        }
        else
        {
            // Help
            Console.Error.WriteLine("ERR: Demo not found");
            foreach(var item in demos.Values)
            {
                Console.WriteLine($" {item.Name,20} - '{item.Desc}'");
            }
            return 1;
        }
    }

}
