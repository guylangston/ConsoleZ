using ConsoleZ.Core.DemoApp;

internal class Program
{
    private static int Main(string[] args)
    {
        Dictionary<string, Action> demos = new();

        demos["bounce"] = ()=>
        {
            var scene = new BouncingBoxScene();
            var app = new ReservedLinesConsoleApp(10, scene);
            var host = new TextApplicationHost(args, app, 10);
            host.Run();
        };

        demos["demo-app"] = ()=>
        {
            var scene = new CountryListScene();
            var app = new ReservedLinesConsoleApp(10, scene);
            var host = new TextApplicationHost(args, app, 10);
            host.Run();
        };

        if (args.Length > 0 && demos.TryGetValue(args.First(), out var action))
        {
            action();
            return 0;
        }
        else
        {
            // Help
            Console.Error.WriteLine("ERR: Demo not found");
            foreach(var key in demos.Keys)
            {
                Console.WriteLine($" --> {key}");
            }
            return 1;
        }
    }
}
