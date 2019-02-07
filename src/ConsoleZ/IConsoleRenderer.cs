using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleZ
{
    public interface IConsoleRenderer
    {
        string RenderLine(string s);
    }

    public class AnsiConsoleRenderer : IConsoleRenderer
    {
        public string RenderLine(string s)
        {
            int i, j;

            // Replace colour tokens in the format ^colorName;
            while((i = s.IndexOf('^')) > 0 && (j = s.IndexOf(';',i)) > 0)
            {
                var ss = s.Substring(i+1, j - i - 1);
                var rep = AnsiConsole.Escape(0);
                if (ss.Length > 0)
                {
                    rep = AnsiConsole.EscapeFore(Color.FromName(ss));
                }
                
                s = s.Remove(i, j - i+1).Insert(i, rep);

            }
            return s;
        }
    }

    public class PlainConsoleRenderer : IConsoleRenderer
    {
        public string RenderLine(string s)
        {
            int i, j;

            // Replace colour tokens in the format ^colorName;
            while((i = s.IndexOf('^')) > 0 && (j = s.IndexOf(';',i)) > 0)
            {
                s = s.Remove(i, j - i+1);
            }
            return s;
        }
    }

    public class HtmlConsoleRenderer : IConsoleRenderer
    {
        public string RenderLine(string s)
        {
            return s;
        }
    }
}
