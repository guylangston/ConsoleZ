using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Console.Playground
{
    internal class Windows10Console
    {
        private static ConsoleInterop.CHAR_INFO[] m_bufScreen;

        // https://pinvoke.net/search.aspx?search=FF_DONTCARE&namespace=[All]
        private const byte FF_DONTCARE = (0 << 4);
        private const ushort FW_NORMAL = 400;

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


            //// Set the size of the screen buffer
            //COORD coord = { (short)m_nScreenWidth, (short)m_nScreenHeight };
            //if (!SetConsoleScreenBufferSize(m_hConsole, coord))
            //    Error(L"SetConsoleScreenBufferSize");
            var coord = new ConsoleInterop.COORD()
            {
                X = (short) m_nScreenWidth,
                Y = (short) m_nScreenHeight
            };
            if (!ConsoleInterop.ConsoleFunctions.SetConsoleScreenBufferSize(m_hConsole, coord))
            {
                throw new Exception("SetConsoleScreenBufferSize");
            }

            //// Assign screen buffer to the console
            //if (!SetConsoleActiveScreenBuffer(m_hConsole))
            //    return Error(L"SetConsoleActiveScreenBuffer");
            if (!ConsoleInterop.ConsoleFunctions.SetConsoleActiveScreenBuffer(m_hConsole))
            {
                throw new Exception("SetConsoleActiveScreenBuffer");
            }

            // Set the font size now that the screen buffer has been assigned to the console
            /*
            CONSOLE_FONT_INFOEX cfi;
            cfi.cbSize = sizeof(cfi);
            cfi.nFont = 0;
            cfi.dwFontSize.X = fontw;
            cfi.dwFontSize.Y = fonth;
            cfi.FontFamily = FF_DONTCARE;
            cfi.FontWeight = FW_NORMAL;
                                  
            wcscpy_s(cfi.FaceName, L"Consolas");
            if (!SetCurrentConsoleFontEx(m_hConsole, false, &cfi))
                return Error(L"SetCurrentConsoleFontEx");
            */
            if (true)
            {
                var fontw = 16;
                var fonth = 16;

                int TMPF_TRUETYPE = 4;
                var font = "Consolas";

                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/c276b9ae-dc4c-484a-9a59-1ee66cf0f1cc/c-changing-console-font-programmatically?forum=csharpgeneral
                unsafe
                {
                    var cfi = new ConsoleInterop.CONSOLE_FONT_INFO_EX()
                    {
                        nFont = 0,
                        dwFontSize = new ConsoleInterop.COORD()
                        {
                            X = (short)fontw,
                            Y = (short)fonth
                        },
                        FontFamily = TMPF_TRUETYPE, //FF_DONTCARE,
                        FontWeight = FW_NORMAL,
                    };

                    IntPtr ptr = new IntPtr(cfi.FaceName);
                    Marshal.Copy(font.ToCharArray(), 0, ptr, font.Length);

                    cfi.cbSize = (ushort)Marshal.SizeOf(cfi);
                    if (!ConsoleInterop.ConsoleFunctions.SetCurrentConsoleFontEx(m_hConsole, false, ref cfi))
                    {
                        var msg = "SetConsoleActiveScreenBuffer:" + Marshal.GetLastWin32Error();
                        throw new Exception(msg);
                    }
                }
            }
            
            ConsoleInterop.CONSOLE_SCREEN_BUFFER_INFO csbi = new ConsoleInterop.CONSOLE_SCREEN_BUFFER_INFO();
            if (!ConsoleInterop.ConsoleFunctions.GetConsoleScreenBufferInfo(m_hConsole, out csbi))
            {
                throw new Exception("GetConsoleScreenBufferInfo");
            }
                
            if (m_nScreenHeight > csbi.dwMaximumWindowSize.Y) throw new Exception("Screen Height / Font Height Too Big");
            if (m_nScreenWidth > csbi.dwMaximumWindowSize.X) throw new Exception("Screen Width / Font Width Too Big");

            
            // Set Physical Console Window Size
            m_rectWindow = new ConsoleInterop.SMALL_RECT()
            {
                Top =  0,
                Left = 0,
                Right = (short)(m_nScreenWidth - 1),
                Bottom = (short)(m_nScreenHeight - 1)
            };
            if (!ConsoleInterop.ConsoleFunctions.SetConsoleWindowInfo(m_hConsole, true, ref m_rectWindow))
            {
                throw new Exception("SetConsoleWindowInfo");
            }
                

            // Allocate memory for screen buffer
            m_bufScreen = new ConsoleInterop.CHAR_INFO[m_nScreenWidth*m_nScreenHeight];

            

            for (int i = 0; i < 2000; i++)
            {
                char x = (char) ((int) 'A' + (i % 26));
                Array.Fill(m_bufScreen, new ConsoleInterop.CHAR_INFO(x, 0x00FD));
                
                UpdateBuffer(m_hConsole, m_nScreenWidth, m_nScreenHeight, m_rectWindow);
            }


        }

        private static void UpdateBuffer(IntPtr m_hConsole, int m_nScreenWidth, int m_nScreenHeight, ConsoleInterop.SMALL_RECT m_rectWindow)
        {
            if (!ConsoleInterop.ConsoleFunctions.WriteConsoleOutput(m_hConsole,
                m_bufScreen,
                new ConsoleInterop.COORD()
                {
                    X = (short) m_nScreenWidth,
                    Y = (short) m_nScreenHeight
                },
                new ConsoleInterop.COORD()
                {
                    X = 0,
                    Y = 0
                },
                ref m_rectWindow))
            {
                throw new Exception("WriteConsoleOutput");
            }

            ;
        }

        // https://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
        const int STD_OUTPUT_HANDLE = -11;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        private static IntPtr OutputHandle => GetStdHandle(STD_OUTPUT_HANDLE);
    }
}