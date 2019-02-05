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
    }
}
