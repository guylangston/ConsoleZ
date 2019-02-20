﻿using System;
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

        protected ConsoleDataBuilder GetBuilder()
        {
            if (builder == null)
            {
                builder = new ConsoleDataBuilder(Url.Action("UpdateConsole") + "/{0}");
            }
            return builder;
        }

        protected string ConsoleHostView { get; set; } = "Console";
        
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult UpdateConsole(string id)
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
        public IActionResult Console(string id)
        {
            if (ConsoleRepository.TryGetConsole(id, out var cons))
            {
                return View(ConsoleHostView, new ConsoleDataModel(cons, GetBuilder().ToDto(cons)));
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