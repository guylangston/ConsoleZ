using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleZ.Internal;

namespace ConsoleZ
{
    public interface IConsoleRenderer
    {
        string RenderLine(IConsole cons, int index, string s);
    }

    public class AnsiConsoleRenderer : IConsoleRenderer
    {
        public string RenderLine(IConsole cons, int index,  string s)
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
        public string RenderLine(IConsole cons, int index, string s)
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

    public class MarkUpToken
    {
        
    }

    public class HtmlConsoleRenderer : TokenParser<MarkUpToken>, IConsoleRenderer
    {

        protected override Token<MarkUpToken> CreateToken(string text,int start, int end, bool isLiteral)
        {
            var raw = text.Substring(start, end - start + 1);
            return new Token<MarkUpToken>()
            {
                Start = start,
                End = end,
                RawText = raw,
                Text = raw.Trim('^', ';', '*'),
                IsLiteral = isLiteral
            };
        }

        protected override bool IsStart(string text, int index, char c)
        {
            if (c == '*' && index < text.Length-1 && text[index + 1] == '*') return true;
            return c == '^';
        }

        protected override bool IsEnd(string text, int start, int index, char endC)
        {
            if (endC == '*' && text[start] == '*') return true;

            return endC == ';';
        }


        public string RenderLine(IConsole cons, int index, string s)
        {
            if (s.StartsWith("# "))
            {
                return $"<h1>{s.Remove(0,2)}</h1>";
            }

            if (string.IsNullOrWhiteSpace(s))
            {
                return "";
            }
            
            Scan(s);
            return Render((i, t) =>
            {
                if (t.IsLiteral) return t.Text;

                if (t.Text == "")
                {
                    return $"</span>";
                }

                if (TryGetPreviousNonLiteral(t, out var pt) && pt.Text != "")
                {
                    return $"</span><span style=\"color:{t.Text};\">";
                }

                return $"<span style=\"color:{t.Text};\">";
            });
        }

        
    }
}
