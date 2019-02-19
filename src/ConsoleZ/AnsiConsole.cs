using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ConsoleZ
{
    /// <summary>
    /// ANSI Terminal Escape Codes
    /// http://www.lihaoyi.com/post/BuildyourownCommandLinewithANSIescapecodes.html
    ///
    /// Enable in Win10
    /// https://www.jerriepelser.com/blog/using-ansi-color-codes-in-net-console-apps/
    ///
    /// Wikipedia: ANSI escape code
    /// https://en.wikipedia.org/wiki/ANSI_escape_code
    /// </summary>
    public sealed class AnsiConsole : ConsoleBase, IConsoleLineRenderer
    {
        private static readonly object locker = new object();
        private static volatile AnsiConsole singleton = null;
        private AnsiConsole() : base("AnsiConsole", Console.WindowWidth, Console.BufferHeight)
        {
            Renderer = new AnsiConsoleRenderer();
        }

        public static AnsiConsole Singleton
        {
            get
            {
                if (singleton != null) return singleton;
                lock (locker)
                {
                    if (singleton == null)
                    {
                        var t = new AnsiConsole();
                        t.EnableANSI();

                        // Sync the current display
                        Console.Clear();

                        singleton = t;
                    }

                    return singleton;
                }
            }
        }

        public override string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }
    
        public override void LineChanged(int index, string line, bool updated)
        {
            if (updated)
            {
                var x = Console.CursorTop;
                Console.CursorTop = index - DisplayStart + (DisplayStart == 0 ? 0 : -1);
                Console.CursorLeft = 0;

                var rline = RenderLine(this, index, line);
                if (rline.Length > Console.WindowWidth )
                {
                    rline = rline.Substring(0, Console.WindowWidth);
                }

                Console.Write(rline.PadRight(Console.WindowWidth - 1));

                Console.CursorTop = x;
                Console.CursorLeft = 0;
            }
            else
            {
                
                Console.WriteLine(RenderLine(this, index,  line));
            }
        }

        public string RenderLine(IConsole cons, int index, string s)
        {
            s = Renderer.RenderLine(cons, index, s);
            return $"{Escape(34)}{index,4} |{Escape(0)} {s}";
        }

        protected override void AddLineCheckWrap(string l)
        {
            if (l != null && l.Length + 6 > Width)
            {
                while (l.Length + 6 > Width)
                {
                    var front = l.Substring(0, Width - 7 );
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

        public static string Escape(int clr) => $"\u001b[{clr}m";
        public static string EscapeFore(Color c) => $"\u001b[38;2;{c.R};{c.G};{c.B}m"; // https://en.wikipedia.org/wiki/ANSI_escape_code#24-bit
        public static string EscapeBack(Color c) => $"\u001b[48;2;{c.R};{c.G};{c.B}m"; // https://en.wikipedia.org/wiki/ANSI_escape_code#24-bit

        public void EnableANSI()
        {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
            {
                throw new Exception("failed to get output console mode");
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(iStdOut, outConsoleMode))
            {
                throw new Exception($"failed to set output console mode, error code: {GetLastError()}");
            }
        }

        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

       
    }
}