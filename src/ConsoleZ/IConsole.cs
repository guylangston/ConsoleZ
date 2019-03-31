using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ConsoleZ
{
    public interface ITextWriter
    {
        int WriteLine(string s);        // We don't use Write, rather update the same line
        int WriteFormatted(FormattableString formatted);
    }

    public interface IConsole : ITextWriter
    {
        string Handle { get; }
        int Version { get;  }

        int Width { get; }
        int Height { get; }
        int DisplayStart { get;  }
        int DisplayEnd { get; }

        void Clear();
        
        /// <returns>false - unable to update</returns>
        bool UpdateLine(int line, string txt);      
        /// <returns>false - unable to update</returns>
        bool UpdateFormatted(int line, FormattableString formatted);
    }

    public interface IConsoleWithProps : IConsole
    {
        string Title { get; set; }
        
        /// <param name="key">Case Insensitive</param>
        void SetProp(string key, string val);

        /// <param name="key">Case Insensitive</param>
        bool TryGetProp(string key, out string val);
    }

    public interface IConsoleLineRenderer
    {
        string RenderLine(IConsole cons, int index, string raw);
    }

    public interface IAbsConsole<T>
    {
        string Handle { get; }
        int Width { get; }
        int Height { get; }
        T this[int x, int y] { get; set; }

        void Fill(T fill);
    }

    public interface IBufferedAbsConsole<T> : IAbsConsole<T>
    {
        void Update();
    }
}
