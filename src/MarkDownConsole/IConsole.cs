using System;

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
}
