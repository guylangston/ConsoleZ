using System;
using System.Collections.Concurrent;

namespace ConsoleZ.Web
{
   
    // DTO
    public class ConsoleData
    {
        // Meta
        public bool IsActive { get; set; }
        public string Handle { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        // State
        public string HtmlContent { get; set; }
        public string UpdateUrl { get; set; }
        public string Version { get; set; }

        // Navigation
        public string DoneUrl { get; set; }
        public string BackUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    public class WebConsole
    {

    }
}
