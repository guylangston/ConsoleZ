using System;
using ConsoleZ.Web;
using Microsoft.AspNetCore.Mvc;

namespace ConsoleZ.AspNetCore
{
    public abstract class ConsoleZControllerBase : Controller
    {
        private ConsoleDataBuilder builder;
        protected IVirtualConsoleRepository ConsoleRepository { get; }
        
        protected ConsoleZControllerBase(IVirtualConsoleRepository consoleRepository)
        {
            this.ConsoleRepository = consoleRepository ?? throw new ArgumentNullException(nameof(consoleRepository));
        }

        protected virtual ConsoleDataBuilder GetBuilder()
        {
            if (builder == null)
            {
                builder = new ConsoleDataBuilder(Url.Action("UpdateConsole") + "/{0}");
            }
            return builder;
        }

        protected string ConsoleHostView { get; set; } = "Console";
        
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public virtual  IActionResult UpdateConsole(string id)
        {
            if (ConsoleRepository.TryGetConsole(id, out var cons))
            {
                return Json(GetBuilder().ToDto(cons));
            }
            else
            {
                return NotFound($"Id not found: {id}");
            }
            
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public virtual IActionResult Console(string id, string src)
        {
            if (ConsoleRepository.TryGetConsole(id, out var cons))
            {
                return View(ConsoleHostView, new ConsoleDataModel(cons, GetBuilder().ToDto(cons)));
            }
            else
            {
                if (src != null)
                {
                    return Redirect(src);
                }
                return NotFound($"Id not found: {id}");
            }
        }

        protected virtual  IActionResult Console(IConsole console, string src = null)
        {
            return RedirectToAction("Console", new {id = console.Handle, src});
        }
    }
}
