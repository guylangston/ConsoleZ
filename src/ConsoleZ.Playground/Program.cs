﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ConsoleZ;
using ConsoleZ.DisplayComponents;
using ConsoleZ.Drawing;
using ConsoleZ.Samples;
using ConsoleZ.Win32;

namespace ConsoleZ.Playground
{
    class Program
    {

        static void Main(string[] args)
        {
            var cons = AnsiConsole.Singleton;
            cons.WriteLine("Hello World");
            cons.WriteLine("Have a ^red;wonderful^; day!");
            var idx = cons.WriteLine("Replace me");
            cons.UpdateLine(idx, "I was replaced. ;-)");
        }

        //static void Main(string[] args)
        //{

        //    RunHeader();

        //    //RunBenchmark();
        //    //RunMarkDownSample();
        //}

        private static void RunHeader()
        {
            //var cons = AnsiConsole.Singleton;
            //cons.WriteLine($"^cyan;TSDB^; Tools (TSDB: ^yellow;{0.1}^;)");
            //cons.WriteLine(null);
            //cons.WriteLine();
            //cons.WriteLine("Done");

            RunMarkDownSample();
            
        }

        private static void RunMarkDownSample()
        {
            var cons = AnsiConsole.Singleton;
            cons.UsePrefix = true;

            using (var fileTxt =
                new BufferedFileConsole(File.CreateText("e:\\Scratch\\console.txt"), "file", cons.Width, cons.Height)
                {
                    Renderer = new PlainConsoleRenderer()
                })
            {

                using (var fileHtml =
                    new BufferedFileConsole(File.CreateText("e:\\Scratch\\console.html"), "file", cons.Width,
                        cons.Height)
                    {
                        
                        Renderer = new HtmlConsoleRenderer()
                    })
                {
                    cons.Parent = fileTxt;
                    
                    fileTxt.Parent = fileHtml;

                    SampleDocuments.DescribeConsole(cons);

                    //foreach (var i in Enumerable.Range(0, 100))
                    //{
                    //    cons.WriteLine(i.ToString());
                    //}

                    //var ok = cons.UpdateLine(1, "XXX");
                    //cons.WriteLine($"Update -1 => {ok}");

                    SampleDocuments.MarkDownBasics(cons);
                    SlowPlayback.LiveElements(cons);
                    SampleDocuments.ColourPalette(cons);

                    
                   

                    SlowPlayback.LiveElementsFast(cons);


                    SampleDocuments.DescribeConsole(cons);
                    //var a = new ProgressBar(cons, "Test Scrolling").Start(100);
                    //for (int i = 0; i < a.ItemsTotal; i++)
                    //{
                    //    a.Increment(i.ToString());
                    //    Thread.Sleep(200);
                    //    cons.WriteLine(i.ToString());
                    //}
                    //a.Stop();
                }
            }
            

           
        }

        private static void RunBenchmark()
        {
            DirectConsole.Setup(80, 30, 16, 16, "Consolas");
            var screen = DirectConsole.Singleton;
            Console.WriteLine(Benchmark(2000, screen));
        }

        private static string Benchmark(int frameCount, IBufferedAbsConsole<CHAR_INFO> screen)
        {
            var r = new Random();
            var stop = new Stopwatch();
            stop.Start();
            for (int i = 0; i < frameCount; i++)
            {
                var x = r.Next(screen.Width);
                var y = r.Next(screen.Height);
                var a = r.Next(26);
                var clr = r.Next(0, 255);

                screen[x, y] = new CHAR_INFO(TestHelper.GetCharOffset('A', a), (ushort)clr);
                screen.Update();

                if (i % 50 == 0)
                {
                    System.Console.Title = i.ToString();
                }
            }

            stop.Stop();
            return $"{frameCount} frames in {stop.Elapsed} at {frameCount / stop.Elapsed.TotalSeconds} FPS";
        }
    }
}
