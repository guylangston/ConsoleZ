using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsoleZ.Playground.Web.Models;
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
            var cons = StaticVirtualConsoleRepository.Singleton.AddConsole(new VirtualConsole(DateTime.Now.Ticks.ToString(), 80, 30));

            builder.RunAsync(cons, x =>
            {
                ConsoleZ.Samples.SlowPlayback.SimpleCounter(x, int.Parse(consoleText));
                x.SetProp("DoneUrl", "/Home/Privacy");
            });
            

            return Json(builder.ToDto(cons));
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
