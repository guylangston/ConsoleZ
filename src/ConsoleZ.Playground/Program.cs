using System;
using System.Diagnostics;
using System.Threading;
using ConsoleZ;
using ConsoleZ.Win32;

namespace ConsoleZ.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectConsole.Setup(80, 30, 16, 16, "Consolas");
            //DirectConsole.Test();

            var screen = DirectConsole.Singleton;

            var frameCount = 2000;
            var frameDelayMs = 10;
            var r = new Random();
            var stop = new Stopwatch();
            stop.Start();
            for (int i = 0; i < frameCount; i++)
            {
                //screen.Fill(new CHAR_INFO(' ', 0));
                var x = r.Next(screen.Width);
                var y = r.Next(screen.Height);
                var a = r.Next(26);

                screen[x, y] = new CHAR_INFO(TestHelper.GetCharOffset('A', a), 0x3F);
                screen.Update();

                if (i % 50 == 0)
                {
                    System.Console.Title = i.ToString();
                }
                

                //Thread.Sleep(frameDelayMs);
            }
            stop.Stop();
            Console.WriteLine($"{frameCount} frames in {stop.Elapsed} at {frameCount / stop.Elapsed.TotalSeconds} FPS");
        }
    }
}
