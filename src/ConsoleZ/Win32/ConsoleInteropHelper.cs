using System;

namespace ConsoleZ.Win32
{
    internal class ConsoleInteropHelper
    {
        // https://pinvoke.net/default.aspx/kernel32/GetStdHandle.html
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_INPUT_HANDLE =  -10;
        
        public static IntPtr Get_STD_OUTPUT_HANDLE => ConsoleInterop.GetStdHandle(STD_OUTPUT_HANDLE);
        public static IntPtr Get_STD_INPUT_HANDLE => ConsoleInterop.GetStdHandle(STD_INPUT_HANDLE);
        
        public static void EnableMouseSupport()
        {
            // https://stackoverflow.com/questions/1944481/console-app-mouse-click-x-y-coordinate-detection-comparison
            const uint ENABLE_MOUSE_INPUT = 0x0010,
                ENABLE_QUICK_EDIT_MODE = 0x0040,
                ENABLE_EXTENDED_FLAGS = 0x0080,
                ENABLE_ECHO_INPUT = 0x0004,
                ENABLE_WINDOW_INPUT = 0x0008; //more
            
            
            var inputHandle = Get_STD_INPUT_HANDLE;
            uint mode = 0;
            ConsoleInterop.GetConsoleMode(inputHandle, out mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE; //disable
            //mode |= ENABLE_WINDOW_INPUT; //enable (if you want)
            mode |= ENABLE_MOUSE_INPUT; //enable
            ConsoleInterop.SetConsoleMode(inputHandle, mode);
        }
    }
}