using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleZ
{
    public abstract class ConsoleBase : IConsole, IFormatProvider, ICustomFormatter
    {
        protected List<string> lines = new List<string>();

        protected ConsoleBase(string handle, int width, int height)
        {
            Handle = handle;
            Width = width;
            Height = height;
        }


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

        public string Handle { get;  }
        
        public int Width { get; protected set; }
        public int Height { get; protected set;}

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
        
        int AddLine(string s)
        {
            var i = lines.Count -1;
            if (s.IndexOf('\n') > 0)
            {
                // slow
                using (var tr = new StringReader(s))
                {
                    string l = null;
                    while ((l = tr.ReadLine()) != null)
                    {
                        lines.Add(l);
                        i++;
                        LineChanged(i, l, false);
                    }
                }
            }
            else
            {
                lines.Add(s);
                i++;
                LineChanged(i, s, false);
            }
            
            return i;
        }

        

        void EditLine(int line, string txt)
        {
            if (txt.IndexOf('\n') > 0)
            {
                throw new NotImplementedException();
            }
            lines[line - DisplayStart] = txt;
            LineChanged(line, txt, true);
        }

        public abstract void LineChanged(int index, string line, bool updated);
        

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