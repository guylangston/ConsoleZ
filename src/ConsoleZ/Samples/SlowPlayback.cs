using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleZ.Samples
{
    public static class SlowPlayback
    {
        public static void SimpleCounter(IConsole cons, int count = 200)
        {
            cons.WriteLine($"Counting from 0 to {count}...");

            var i = cons.WriteLine($"Testing");
            for (int x = 0; x < count; x++)
            {
                cons.UpdateLine(i, $"Testing {x}");
                Thread.Sleep(200);
            }
            cons.WriteLine($"Counting from 0 to {count}...Done.");
        }

        public static void LiveElements(IConsole cons)
        {
            cons.WriteLine($"Well, {1234}...");
            var t = cons.WriteLine($"Hello");
            cons.WriteLine($"World!");

            cons.WriteLine("\u001b[31mHello World!\u001b[0m");
            cons.WriteLine("\u001b[1m BOLD \u001b[0m\u001b[4m Underline \u001b[0m\u001b[7m Reversed \u001b[0m");
            
            cons.UpdateLine(t, "MyWorld");
            
            cons.WriteLine($"Concurrent Test....");

            var a = cons.WriteLine($"A");
            var b = cons.WriteLine($"B");
            cons.WriteLine($"End Line");
            cons.WriteLine($"");

            //https://en.wikipedia.org/wiki/Box-drawing_character
            cons.WriteLine($"Sample List:");
            cons.WriteLine($"├ List items 1");
            cons.WriteLine($"├ List items 2");
            cons.WriteLine($"└ List items 3");

            Task.Run(() =>
            {
                var p = 20;
                for (int i = 0; i < p; i++)
                {
                    cons.UpdateLine(a, $"Percentage: {i * 100 / p}%");
                    Thread.Sleep(200);
                }
            });
            Task.Run(() =>
            {
                var p = 10;
                for (int i = 0; i < p; i++)
                {
                    cons.UpdateLine(b, $"Percentage: {i * 100 / p}%");
                    Thread.Sleep(200);
                }
            });

            cons.WriteLine($"End Line");
            cons.WriteLine($"End Line");
            cons.WriteLine($"End Line");

            Thread.Sleep(2000);
            foreach (var i in Enumerable.Range(0, 200))
            {
                cons.WriteLine($"Testing scrolling: {i}");
                Thread.Sleep(200);
                cons.UpdateLine(a, $"Off Screen update {i}");
            }
        }

    }
}
