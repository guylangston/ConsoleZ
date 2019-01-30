using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Console.Playground
{
 
    public static class DirectConsole
    {
        // https://github.com/dotnet/corefx/tree/master/src/System.Console
        // https://github.com/dotnet/corefx/blob/master/src/System.Console/src/System/ConsolePal.Windows.cs

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <param name="fontWidth"></param>
        /// <param name="fontHeight"></param>
        public static void Setup(int screenWidth, int screenHeight, int fontWidth, int fontHeight, string font)
        {
            //var m_hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
            m_hConsole = ConsoleStdOutputHandle;

            m_rectWindow = new ConsoleInterop.SMALL_RECT() { Left = 0, Top = 0, Right = 1, Bottom = 1 };
            ConsoleInterop.ConsoleFunctions.SetConsoleWindowInfo(m_hConsole, true, ref m_rectWindow);


            //// Set the size of the screen buffer
            //COORD coord = { (short)m_nScreenWidth, (short)m_nScreenHeight };
            //if (!SetConsoleScreenBufferSize(m_hConsole, coord))
            //    Error(L"SetConsoleScreenBufferSize");
            screenSize = new ConsoleInterop.COORD()
            {
                X = (short)screenWidth,
                Y = (short)screenHeight
            };
            if (!ConsoleInterop.ConsoleFunctions.SetConsoleScreenBufferSize(m_hConsole, screenSize))
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
                

                int TMPF_TRUETYPE = 4;
                

                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/c276b9ae-dc4c-484a-9a59-1ee66cf0f1cc/c-changing-console-font-programmatically?forum=csharpgeneral
                unsafe
                {
                    var cfi = new ConsoleInterop.CONSOLE_FONT_INFO_EX()
                    {
                        nFont = 0,
                        dwFontSize = new ConsoleInterop.COORD()
                        {
                            X = (short)fontWidth,
                            Y = (short)fontHeight
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

            if (screenHeight > csbi.dwMaximumWindowSize.Y) throw new Exception("Screen Height / Font Height Too Big");
            if (screenWidth > csbi.dwMaximumWindowSize.X) throw new Exception("Screen Width / Font Width Too Big");


            // Set Physical Console Window Size
            m_rectWindow = new ConsoleInterop.SMALL_RECT()
            {
                Top = 0,
                Left = 0,
                Right = (short)(screenWidth - 1),
                Bottom = (short)(screenHeight - 1)
            };
            if (!ConsoleInterop.ConsoleFunctions.SetConsoleWindowInfo(m_hConsole, true, ref m_rectWindow))
            {
                throw new Exception("SetConsoleWindowInfo");
            }


            // Allocate memory for screen buffer
            m_bufScreen = new ConsoleInterop.CHAR_INFO[screenWidth * screenHeight];

           
        }

        public static void Test(int frameCount = 2000, int frameDelayMs = 100)
        {
            for (int i = 0; i < frameCount; i++)
            {
                char x = (char)((int)'A' + (i % 26));
                Array.Fill(m_bufScreen, new ConsoleInterop.CHAR_INFO(x, (byte)(i / 10)));

                UpdateBuffer();

                Thread.Sleep(frameDelayMs);
            }
        }


        // https://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
        const int STD_OUTPUT_HANDLE = -11;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        private static IntPtr ConsoleStdOutputHandle => GetStdHandle(STD_OUTPUT_HANDLE);

        private static ConsoleInterop.CHAR_INFO[] m_bufScreen;
        private static IntPtr m_hConsole;
        private static ConsoleInterop.SMALL_RECT m_rectWindow;
        private static ConsoleInterop.COORD screenSize;

        // https://pinvoke.net/search.aspx?search=FF_DONTCARE&namespace=[All]
        private const byte FF_DONTCARE = (0 << 4);
        private const ushort FW_NORMAL = 400;

        private static void UpdateBuffer()
        {
            if (!ConsoleInterop.ConsoleFunctions.WriteConsoleOutput(m_hConsole,
                m_bufScreen,
                screenSize,
                new ConsoleInterop.COORD()
                {
                    X = 0,
                    Y = 0
                },
                ref m_rectWindow))
            {
                throw new Exception("WriteConsoleOutput");
            }
        }
    }
}