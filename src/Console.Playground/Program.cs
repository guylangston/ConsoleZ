using System;
using System.Threading;
using MarkDownConsole;

namespace Console.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectConsole.Setup(80, 30, 16, 16, "Consolas");
            //DirectConsole.Test();

            var frameCount = 2000;
            var frameDelayMs = 10;
            for (int i = 0; i < frameCount; i++)
            {
                DirectConsole.Fill(' ', 0);
                DirectConsole.SetPixel(
                    i % DirectConsole.ScreenWidth,
                    i % DirectConsole.ScreenHeight,
                    '#',
                    0x3F);

                DirectConsole.Update();

                System.Console.Title = i.ToString();

                Thread.Sleep(frameDelayMs);
            }
        }
    }
}
