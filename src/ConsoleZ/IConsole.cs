using System;
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

        int Width { get; }
        int Height { get; }
        int DisplayStart { get;  }
        int DisplayEnd { get; }

        void UpdateLine(int line, string txt);
        void UpdateFormatted(int line, FormattableString formatted);
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
