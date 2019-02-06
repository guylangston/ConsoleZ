using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ConsoleZ.Playground.Web.Models;
using ConsoleZ.Web;


namespace ConsoleZ.Playground.Web.Controllers
{
    public class HomeController : Controller
    {
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
            
            var ret = new ConsoleData()
            {
                Handle = cons.Handle,
                HtmlContent = "Loading...",
                UpdateUrl = Url.Action("ConsoleUpdate", new{id=cons.Handle})
            };

            Task.Run(() =>
            {
                ConsoleZ.Samples.SlowPlayback.SimpleCounter(cons);
            });

            return Json(ret);
        }

        

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ConsoleUpdate(string id)
        {
            if (StaticVirtualConsoleRepository.Singleton.TryGetConsole(id, out var cons))
            {
                return Json(new ConsoleData()
                {
                    IsActive = true,
                    Handle = cons.Handle.ToString(),
                    HtmlContent = string.Join(Environment.NewLine, cons.GetTextLines()),
                    Version = cons.Version.ToString()
                });
            }
            else
            {
                return NotFound(id);
            }
            
        }
    }
}
