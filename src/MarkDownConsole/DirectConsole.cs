using System;
using System.Runtime.InteropServices;
using System.Threading;
using ConsoleZ.Win32;


namespace ConsoleZ
{
 
    // https://github.com/dotnet/corefx/tree/master/src/System.Console
    // https://github.com/dotnet/corefx/blob/master/src/System.Console/src/System/ConsolePal.Windows.cs
    public static class DirectConsole
    {

        // https://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
        const int STD_OUTPUT_HANDLE = -11;
        
        private static IntPtr ConsoleStdOutputHandle => ConsoleInterop.GetStdHandle(STD_OUTPUT_HANDLE);
        private static CHAR_INFO[] m_bufScreen;
        private static IntPtr m_hConsole;
        private static SMALL_RECT m_rectWindow;
        private static COORD screenSize;
        
        // https://pinvoke.net/search.aspx?search=FF_DONTCARE&namespace=[All]
        private const byte FF_DONTCARE = (0 << 4);
        private const ushort FW_NORMAL = 400;

        

        /// <summary>
        /// Setup screen size and direct-access buffer
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <param name="fontWidth"></param>
        /// <param name="fontHeight"></param>
        public static void Setup(int screenWidth, int screenHeight, int fontWidth, int fontHeight, string font)
        {
            //var m_hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
            m_hConsole = ConsoleStdOutputHandle;

            m_rectWindow = new SMALL_RECT() { Left = 0, Top = 0, Right = 1, Bottom = 1 };
            ConsoleInterop.SetConsoleWindowInfo(m_hConsole, true, ref m_rectWindow);

            //// Set the size of the screen buffer
            //COORD coord = { (short)m_nScreenWidth, (short)m_nScreenHeight };
            //if (!SetConsoleScreenBufferSize(m_hConsole, coord))
            //    Error(L"SetConsoleScreenBufferSize");
            screenSize = new COORD()
            {
                X = (short)screenWidth,
                Y = (short)screenHeight
            };
            if (!ConsoleInterop.SetConsoleScreenBufferSize(m_hConsole, screenSize))
            {
                throw new Exception("SetConsoleScreenBufferSize");
            }

            //// Assign screen buffer to the console
            //if (!SetConsoleActiveScreenBuffer(m_hConsole))
            //    return Error(L"SetConsoleActiveScreenBuffer");
            if (!ConsoleInterop.SetConsoleActiveScreenBuffer(m_hConsole))
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
            int TMPF_TRUETYPE = 4;

            // https://social.msdn.microsoft.com/Forums/vstudio/en-US/c276b9ae-dc4c-484a-9a59-1ee66cf0f1cc/c-changing-console-font-programmatically?forum=csharpgeneral
            unsafe
            {
                var cfi = new CONSOLE_FONT_INFO_EX()
                {
                    nFont = 0,
                    dwFontSize = new COORD()
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
                if (!ConsoleInterop.SetCurrentConsoleFontEx(m_hConsole, false, ref cfi))
                {
                    var msg = "SetConsoleActiveScreenBuffer:" + Marshal.GetLastWin32Error();
                    throw new Exception(msg);
                }
            }

            var csbi = new CONSOLE_SCREEN_BUFFER_INFO();
            if (!ConsoleInterop.GetConsoleScreenBufferInfo(m_hConsole, out csbi))
            {
                throw new Exception("GetConsoleScreenBufferInfo");
            }

            if (screenHeight > csbi.dwMaximumWindowSize.Y) throw new Exception("Screen Height / Font Height Too Big");
            if (screenWidth > csbi.dwMaximumWindowSize.X) throw new Exception("Screen Width / Font Width Too Big");

            // Set Physical Console Window Size
            m_rectWindow = new SMALL_RECT()
            {
                Top = 0,
                Left = 0,
                Right = (short)(screenWidth - 1),
                Bottom = (short)(screenHeight - 1)
            };
            if (!ConsoleInterop.SetConsoleWindowInfo(m_hConsole, true, ref m_rectWindow))
            {
                throw new Exception("SetConsoleWindowInfo");
            }

            // Allocate memory for screen buffer
            m_bufScreen = new CHAR_INFO[screenWidth * screenHeight];
        }

        public static CHAR_INFO[] ScreenBuffer => m_bufScreen;

        public static void Test(int frameCount = 2000, int frameDelayMs = 100)
        {
            for (int i = 0; i < frameCount; i++)
            {
                char x = (char)((int)'A' + (i % 26));
                Fill(x, (byte) (i / 10));

                Update();

                Thread.Sleep(frameDelayMs);
            }
        }

        public static void Fill(char c, byte clr)
        {
            for (int i = 0; i < m_bufScreen.Length; i++)
            {
                m_bufScreen[i] = new CHAR_INFO(c, clr);
            }
        }


        public static int ScreenWidth => screenSize.X;
        public static int ScreenHeight => screenSize.Y;

        public static void Set(int x, int y, char c, byte clr) =>
            m_bufScreen[y * ScreenWidth + x] = new CHAR_INFO(c, clr);

        public static (char c, byte clr) Get(int x, int y, char c, byte clr)
        {
            var v = m_bufScreen[y * ScreenWidth + x];
            return (v.UnicodeChar, (byte)v.Attributes);
        }
             

        public static void Update()
        {
            if (!ConsoleInterop.WriteConsoleOutput(m_hConsole,
                m_bufScreen,
                screenSize,
                new COORD()
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