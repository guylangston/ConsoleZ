using System;
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
    /// </summary>
    public sealed class AnsiConsole : ConsoleBase
    {
        private static readonly object locker = new object();
        private static AnsiConsole singleton = null;
        private AnsiConsole()
        {
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
                        singleton = t;
                    }

                    return singleton;
                }
            }
        }
       


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

        protected override  int AddLine(string s)
        {
            var i = base.AddLine(s);
            Console.WriteLine(Render(i,  s));
            return i;
        }

        protected override void EditLine(int line, string txt)
        {
            base.EditLine(line, txt);

            var x = Console.CursorTop;
            Console.CursorTop = line - DisplayStart;
            Console.CursorLeft = 0;

            Console.Write(Render(line, txt).PadRight(Console.WindowWidth - 1));

            Console.CursorTop = x;
            Console.CursorLeft = 0;
        }

    }
}