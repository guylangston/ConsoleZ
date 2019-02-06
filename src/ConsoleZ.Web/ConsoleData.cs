using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleZ.Web
{
   
    // DTO
    public class ConsoleData
    {
        // Meta
        public string Handle { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Version { get; set; }

        // Output / Render
        public string HtmlContent { get; set; }

        // State
        public bool IsActive { get; set; }
        public string UpdateUrl { get; set; }
       

        // Navigation
        public string DoneUrl { get; set; }
        public string BackUrl { get; set; }
        public string CancelUrl { get; set; }

        public Dictionary<string, string> Props { get; set; }
    }

    public class ConsoleDataBuilder
    {
        private string urlTemplate;
        private Func<IConsole, string> renderHtml;

        public ConsoleDataBuilder(string urlTemplate, Func<IConsole, string> renderHtml)
        {
            this.urlTemplate = urlTemplate;
            this.renderHtml = renderHtml;
        }

        public ConsoleDataBuilder(string urlTemplate)
        {
            this.urlTemplate = urlTemplate;
            renderHtml = DefaultRenderer;
        }

        public static string DefaultRenderer(IConsole cons)
        {
            if (cons is VirtualConsole vCons)
            {
                return string.Join(Environment.NewLine, vCons.GetTextLines());
            }
            
            return $"Not Supported: {cons.GetType().Name}";
        }


        public ConsoleData ToDto(IConsole cons)
        {
            if (cons == null) throw new ArgumentNullException(nameof(cons));
            if (cons.Handle == null) throw new ArgumentNullException(nameof(cons.Handle));

            var dto = new ConsoleData()
            {
                Handle = cons.Handle,
                Width = cons.Width,
                Height = cons.Height,
                Version = cons.Version,
                
                UpdateUrl = string.Format(urlTemplate, cons.Handle),
                HtmlContent = renderHtml(cons)
            };

            if (cons is IConsoleWithProps consProps)
            {
                if (consProps.TryGetProp("IsActive", out var active))
                {
                    dto.IsActive = bool.Parse(active);
                }

                if (consProps.TryGetProp("DoneUrl", out var done)) dto.DoneUrl = done;
                if (consProps.TryGetProp("BackUrl", out var back)) dto.BackUrl = back;
                if (consProps.TryGetProp("CancelUrl", out var cancel)) dto.CancelUrl = cancel;
            }

            return dto;
        }

        public Task RunAsync(IConsoleWithProps cons, Action<IConsoleWithProps> action)
        {
            return Task.Run(() =>
            {
                try
                {
                    cons.SetProp("IsActive", true.ToString());
                    action(cons);
                }
                catch (Exception e)
                {
                    cons.WriteLine(e.ToString());
                    cons.SetProp("Error", e.GetType().Name);
                }
                finally
                {
                    cons.SetProp("IsActive", false.ToString());
                }

            });
        }
    }
}
