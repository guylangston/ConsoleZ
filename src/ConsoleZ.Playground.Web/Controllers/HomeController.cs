using System;
using System.Diagnostics;
using System.Threading;
using ConsoleZ.AspNetCore;
using ConsoleZ.DisplayComponents;
using Microsoft.AspNetCore.Mvc;
using ConsoleZ.Playground.Web.Models;
using ConsoleZ.Samples;
using Microsoft.AspNetCore.Http;


namespace ConsoleZ.Playground.Web.Controllers
{
    
    public class HomeController : ConsoleZControllerBase
    {
        public HomeController() : base(StaticVirtualConsoleRepository.Singleton)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult ConsoleStart(string consoleText)
        {
            var consx = ConsoleRepository.CreateConsole();

            consx.WriteLine($"Starting command '{consoleText}'... ");

            GetBuilder().RunAsync(consx, cons =>
            {
                SampleDocuments.MarkDownBasics(cons);
                SlowPlayback.LiveElements(cons);
                SampleDocuments.ColourPalette(cons);

                if (consoleText == "err")
                {
                    throw new Exception("Sample Error");
                }

                var a = new ProgressBar(cons, "Test Scrolling").Start(100);
                for (int i = 0; i < a.ItemsTotal; i++)
                {
                    a.Increment(i.ToString());
                    Thread.Sleep(200);
                }
                a.Stop();

               

                cons.SetProp("DoneUrl", "/Home/Privacy");
            });

            return Console(consx);
        }

        

       
    }
}