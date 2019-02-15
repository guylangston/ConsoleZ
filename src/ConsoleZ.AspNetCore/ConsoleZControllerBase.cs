using System;
using ConsoleZ.Web;
using Microsoft.AspNetCore.Mvc;

namespace ConsoleZ.AspNetCore
{
    public abstract class ConsoleZControllerBase : Controller
    {
        protected readonly ConsoleDataBuilder builder;
        private readonly IVirtualConsoleRepository consoleRepository;
        
        protected ConsoleZControllerBase(string urlTemplate, IVirtualConsoleRepository consoleRepository)
        {
            if (urlTemplate == null) throw new ArgumentNullException(nameof(urlTemplate));
            this.consoleRepository = consoleRepository;
            builder = new ConsoleDataBuilder(urlTemplate);
        }

        protected string ConsoleHostView { get; set; } = "Console";
        
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult UpdateConsole(string id)
        {
            if (consoleRepository.TryGetConsole(id, out var cons))
            {
                return Json(builder.ToDto(cons));
            }
            else
            {
                return NotFound($"Id not found: {id}");
            }
            
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Console(string id)
        {
            if (consoleRepository.TryGetConsole(id, out var cons))
            {
                return View(ConsoleHostView, new ConsoleDataModel(cons, builder.ToDto(cons)));
            }
            else
            {
                return NotFound($"Id not found: {id}");
            }
        }

        protected IActionResult Console(IConsole console)
        {
            return RedirectToAction("Console", new {id = console.Handle});
        }
    }
}
