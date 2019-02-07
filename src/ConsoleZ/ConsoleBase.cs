using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using ConsoleZ.Win32;

namespace ConsoleZ
{
    // TODO: MaxLines and off-screen rules.
    public abstract class ConsoleBase : IConsoleWithProps, IFormatProvider, ICustomFormatter
    {
        protected List<string> lines = new List<string>();
        private ConcurrentDictionary<string, string> props = new ConcurrentDictionary<string, string>();

        protected ConsoleBase(string handle, int width, int height)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            Width = width;
            Height = height;
            Version = 0;
            Renderer = new PlainConsoleRenderer();
        }


        public int WriteLine(string s)
        {
            lock (this)
            {
                return AddLineCheckLineFeed(s);
            }
        }

        public int WriteFormatted(FormattableString formatted)
        {
            lock (this)
            {
                return AddLineCheckLineFeed(formatted.ToString(this));
            }
        }

        public string Handle { get;  }
        public int Version { get; private set; }
        
        public int Width { get; protected set; }
        public int Height { get; protected set;}

        public int DisplayStart { get; private set; }
        public int DisplayEnd => lines.Count;

        protected IConsoleRenderer Renderer { get; set; }

        public virtual string Title { get; set; }

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
        
        int AddLineCheckLineFeed(string s)
        {
            Version++;
            if (s.IndexOf('\n') > 0)
            {
                // slow
                using (var tr = new StringReader(s))
                {
                    string l = null;
                    while ((l = tr.ReadLine()) != null)
                    {
                        AddLineCheckWrap(l);
                    }
                }
            }
            else
            {
                AddLineCheckWrap(s);
            }
            return lines.Count - 1;
        }

        protected virtual void AddLineCheckWrap(string l)
        {
            if (l.Length > Width)
            {
                while (l.Length > Width)
                {
                    var front = l.Substring(0, Width);
                    AddLineInner(front);
                    l = l.Remove(0, front.Length);
                }

                if (l.Length > 0)
                {
                    AddLineInner(l);
                }
                return;
            }
            else
            {
                AddLineInner(l);
            }
        }

        protected void AddLineInner(string l)
        {
            lines.Add(l);

            
            LineChanged(lines.Count -1 , l, false);

            // Check screen up
            if (lines.Count - DisplayStart > Height)
            {
                ScrollUp();
            }
        }

      

        protected virtual void ScrollUp()
        {
            DisplayStart++;
        }


        void EditLine(int line, string txt)
        {
            if (txt.IndexOf('\n') > 0)
            {
                throw new NotImplementedException();
            }
            lines[line - DisplayStart] = txt;
            Version++;
            LineChanged(line, txt, true);
        }

        public abstract void LineChanged(int index, string line, bool updated);
        
        public object GetFormat(Type formatType) => this;

        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            return $"[{arg}]";
        }

        
        public void SetProp(string key, string val) => props[key.ToLowerInvariant()] = val;
        public bool TryGetProp(string key, out string val) => props.TryGetValue(key.ToLowerInvariant(), out val);
    }
}