using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ConsoleZ.DisplayComponents;
using Microsoft.AspNetCore.Mvc;
using ConsoleZ.Playground.Web.Models;
using ConsoleZ.Samples;
using ConsoleZ.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace ConsoleZ.Playground.Web.Controllers
{
    public class HomeController : Controller
    {
        ConsoleDataBuilder builder = new ConsoleDataBuilder("/Home/ConsoleUpdate/{0}");

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
            var consx = StaticVirtualConsoleRepository.Singleton.AddConsole(new VirtualConsole(DateTime.Now.Ticks.ToString(), 80, 30));

            builder.RunAsync(consx, cons =>
            {
                cons.WriteLine($"Starting command '{consoleText}'... ");

                SampleDocuments.MarkDownBasics(cons);
                SlowPlayback.LiveElements(cons);
                SampleDocuments.ColourPalette(cons);

                var a = new ProgressBar(cons, "Test Scrolling").Start(100);
                for (int i = 0; i < a.ItemsTotal; i++)
                {
                    a.Increment(i.ToString());
                    Thread.Sleep(200);
                }
                a.Stop();

                cons.SetProp("DoneUrl", "/Home/Privacy");
            });
            

            return Json(builder.ToDto(consx));
        }

        

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ConsoleUpdate(string id)
        {
            if (StaticVirtualConsoleRepository.Singleton.TryGetConsole(id, out var cons))
            {
                return Json(builder.ToDto(cons));
            }
            else
            {
                return NotFound(id);
            }
            
        }
    }
}