using System;
using System.Threading;
using System.Threading.Tasks;
using MarkDownConsole;

namespace Console.Playground
{
    class Program
    {
        static void Main(string[] args)
        {

            
            
            var cons = new Console2();
            cons.EnableANSI();

            cons.WriteLine($"Well, {1234}...");
            var t = cons.WriteLine($"Hello");
            cons.WriteLine($"World!");

            cons.WriteLine("\u001b[31mHello World!\u001b[0m");
            cons.WriteLine("\u001b[1m BOLD \u001b[0m\u001b[4m Underline \u001b[0m\u001b[7m Reversed \u001b[0m");


            
            Thread.Sleep(1000);

            cons.UpdateLine(t, "MyWorld");


            cons.WriteLine($"Concurrent Test....");

            var a = cons.WriteLine($"A");
            var b = cons.WriteLine($"B");
            cons.WriteLine($"End Line");
            cons.WriteLine($"End Line");

            Task.Run(() =>
            {
                var p = 40;
                for (int i = 0; i < p; i++)
                {
                    cons.UpdateLine(a, $"Percentage: {i * 100 / p:0.00}%");
                    Thread.Sleep(100);
                }
            });
            Task.Run(() =>
            {
                var p = 140;
                for (int i = 0; i < p; i++)
                {
                    cons.UpdateLine(b, $"Percentage: {i * 100 / p:0.00}%");
                    Thread.Sleep(200);
                }
            });


            System.Console.ReadLine();
        }
    }
}
