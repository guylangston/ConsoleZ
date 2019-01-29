using System;
using System.Collections.Generic;

namespace MarkDownConsole
{
    public abstract class ConsoleBase : IConsole, IFormatProvider, ICustomFormatter
    {
        protected List<string> lines = new List<string>();
        
        public int WriteLine(string s)
        {
            lock (this)
            {
                return AddLine(s);
            }
        }

        public int WriteFormatted(FormattableString formatted)
        {
            lock (this)
            {
                return AddLine(formatted.ToString(this));
            }
        }

        public string Handle { get; private set; }
        
        public int Width { get; private set; }
        public int Height { get; private set;}

        public int DisplayStart { get; }
        public int DisplayEnd { get; }

        public void UpdateLine(int line, string txt)
        {
            lock (this)
            {
                EditLine(line, txt);
            }
        }

        public void UpdateFormatted(int line, FormattableString formatted)
        {
            lock (this)
            {
                EditLine(line, formatted.ToString(this));
            }
        }
        
        protected virtual int AddLine(string s)
        {
            lines.Add(s);
            return lines.Count - 1;
        }

        protected virtual void EditLine(int line, string txt)
        {
            lines[line - DisplayStart] = txt;
        }

        public object GetFormat(Type formatType) => this;

        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            return $"[{arg}]";
        }

        protected virtual string Render(int line, string s)
        {
            return $"{Escape(34)}{line,4} |{Escape(0)} {s}";
        }

        public string Escape(int clr) => $"\u001b[{clr}m";
    }
}