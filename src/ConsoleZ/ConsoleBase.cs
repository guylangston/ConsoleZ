using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ConsoleZ.DisplayComponents;

namespace ConsoleZ
{
    // TODO: MaxLines and off-screen rules.
    public abstract class ConsoleBase : IConsoleWithProps, IFormatProvider, ICustomFormatter
    {
        private readonly ConcurrentDictionary<string, string> props = new ConcurrentDictionary<string, string>();
        protected List<string> lines = new List<string>();

        protected ConsoleBase(string handle, int width, int height)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            Width = width;
            Height = height;
            Version = 0;
            Renderer = new PlainConsoleRenderer();
            Formatter = this;
        }

        public IConsole Parent { get; set; }

        public ICustomFormatter Formatter { get; set; } 
        public IConsoleRenderer Renderer { get; set; }
        
        public string Handle { get; }
        public int Version { get; private set; }

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public int DisplayStart { get; private set; }
        public int DisplayEnd => lines.Count;

        public virtual string Title { get; set; }

        public int WriteLine(string s)
        {
            Parent?.WriteLine(s);
            lock (this)
            {
                return AddLineCheckLineFeed(s);
            }
        }

        public int WriteFormatted(FormattableString formatted)
        {
            Parent?.WriteFormatted(formatted);
            lock (this)
            {
                return AddLineCheckLineFeed(formatted.ToString(this));
            }
        }

        public virtual void Clear()
        {
            DisplayStart = 0;
        }

        public bool UpdateLine(int line, string txt)
        {
            Parent?.UpdateLine(line, txt);
            lock (this)
            {
                return EditLine(line, txt);
            }
        }

        public bool UpdateFormatted(int line, FormattableString formatted)
        {
            Parent?.UpdateFormatted(line, formatted);
            lock (this)
            {
                return EditLine(line, formatted.ToString(this));
            }
        }
        
        public void SetProp(string key, string val)
        {
            props[key.ToLowerInvariant()] = val;
        }

        public bool TryGetProp(string key, out string val)
        {
            return props.TryGetValue(key.ToLowerInvariant(), out val);
        }

        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null) return null;

            if (arg is DateTime dt)
            {
                return dt.ToString("yyyy-MM-dd");
            }
            if (arg is TimeSpan sp)
            {
                return ProgressBar.Humanize(sp);
            }

            return arg.ToString();
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }

        private int AddLineCheckLineFeed(string s)
        {
            Version++;
            if (s != null && s.IndexOf('\n') > 0)
                using (var tr = new StringReader(s))
                {
                    string l = null;
                    while ((l = tr.ReadLine()) != null) AddLineCheckWrap(l);
                }
            else
                AddLineCheckWrap(s);

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

                if (l.Length > 0) AddLineInner(l);
            }
            else
            {
                AddLineInner(l);
            }
        }


        protected void AddLineInner(string l)
        {
            // ReSharper disable InconsistentlySynchronizedField
            lines.Add(l);
            LineChanged(lines.Count - 1, l, false);

            // Check screen up
            if (lines.Count - DisplayStart > Height) ScrollUp();
            // ReSharper restore InconsistentlySynchronizedField
        }

        protected virtual void ScrollUp()
        {
            DisplayStart++;
        }


        private bool EditLine(int line, string txt)
        {
            if (txt.IndexOf('\n') > 0) throw new NotImplementedException();

            var index = line - DisplayStart;
            if (index > 0 && lines.Count > index)
            {
                lines[index] = txt;
                Version++;
                LineChanged(line, txt, true);
                return true;
            }

            return false;
        }

        public abstract void LineChanged(int index, string line, bool updated);
    }
}