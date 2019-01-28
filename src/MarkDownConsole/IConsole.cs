using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MarkDownConsole
{
    public interface ITextWriter
    {
        //int WriteLine(string s);        // We don't use Write
        //int WriteLine(For);
    }

    public interface IConsole : ITextWriter
    {
        string Handle { get; }
        int Width { get; }
        int Height { get; }
        int StartLine { get;  }
        int LastLine { get; }

        void UpdateLine(int line, string format, params object[] args);
    }

    public abstract class ConsoleBase : IConsole, IFormatProvider, ICustomFormatter
    {
        protected List<string> lines = new List<string>();

        public int WriteLine(FormattableString fm)
        {
            var s = fm.ToString(this);
            return WriteLine(s);
        }


        public int WriteLine(string s)
        {
            lock (this)
            {

                return AddLine(s);
            }
        }


        protected virtual int AddLine(string s)
        {
            lines.Add(s);
            return lines.Count - 1;
        }

   

        public string Handle { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set;}
        public int StartLine { get; private set; }
        public int LastLine { get; private set; }

        public virtual void UpdateLine(int line, string format, params object[] args)
        {
            lock (this)
            {
                EditLine(line, format, args);
            }
        }

        protected virtual void EditLine(int line, string format, object[] args)
        {
            lines[line - StartLine] = string.Format(this, format, args);
        }

        public object GetFormat(Type formatType) => this;

        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            return $"[{arg}]";
        }
    }

    public class Console2 : ConsoleBase
    {

        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public Console2()
        {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                Console.WriteLine("failed to get output console mode");
                Console.ReadKey();
                return;
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(iStdOut, outConsoleMode))
            {
                Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }           

            
        }

        protected override  int AddLine(string s)
        {
            var i = base.AddLine(s);

            Console.WriteLine(lines[i]);
            return i;
        }

        protected override void EditLine(int line, string format, params object[] args)
        {
            base.EditLine(line, format, args);

            var x = Console.CursorTop;
            Console.CursorTop = line - StartLine;
            Console.CursorLeft = 0;
            Console.Write(lines[line].PadRight(Console.WindowWidth-1));
            Console.CursorTop = x;
            Console.CursorLeft = 0;
        }

    }
}
