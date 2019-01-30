using System;
using MarkDownConsole;

namespace Console.Playground
{
    class Program
    {
        static void Main(string[] args)
        {

            //    var full = new Console2();
            //    full.EnableANSI();

            //    TestHelper.WriteScenarioA(full);

            DirectConsole.Setup(80, 30, 16, 16, "Consolas");
            DirectConsole.Test();
        }
    }
}
