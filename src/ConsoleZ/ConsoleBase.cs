using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace ConsoleZ
{
    // TODO: MaxLines and off-screen rules.
    public abstract class ConsoleBase : IConsoleWithProps, IFormatProvider, ICustomFormatter
    {
        protected List<string> lines = new List<string>();
        private ConcurrentDictionary<string, string> props = new ConcurrentDictionary<string, string>();
        protected int version;

        protected ConsoleBase(string handle, int width, int height)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            Width = width;
            Height = height;
            version = 0;
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
        public int Version => version;
        
        public int Width { get; protected set; }
        public int Height { get; protected set;}

        public int DisplayStart { get; }
        public int DisplayEnd { get; }

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
        
        int AddLine(string s)
        {
            var i = lines.Count -1;
            version++;
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
            version++;
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