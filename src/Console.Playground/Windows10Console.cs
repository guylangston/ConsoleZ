using System;
using System.Runtime.InteropServices;

namespace Console.Playground
{
    internal class Windows10Console
    {
        // https://github.com/dotnet/corefx/tree/master/src/System.Console
        // https://github.com/dotnet/corefx/blob/master/src/System.Console/src/System/ConsolePal.Windows.cs
        internal static void Plaground()
        {
            var m_nScreenWidth = 80;
            var m_nScreenHeight = 30;

            //var m_hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
            var m_hConsole = OutputHandle;

            ConsoleInterop.SMALL_RECT m_rectWindow =  new ConsoleInterop.SMALL_RECT() { Left = 0, Top=0, Right = 1, Bottom = 1 };
            ConsoleInterop.ConsoleFunctions.SetConsoleWindowInfo(m_hConsole, true, ref m_rectWindow);
        }

        // https://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
        const int STD_OUTPUT_HANDLE = -11;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        private static IntPtr OutputHandle => GetStdHandle(STD_OUTPUT_HANDLE);
    }
}