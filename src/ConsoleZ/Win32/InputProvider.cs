using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VectorInt;

namespace ConsoleZ.Win32
{
    public class InputProvider : IDisposable
    {
        private Task background;

        public InputProvider()
        {
            CancellationToken = new CancellationToken();
            background = Task.Run(ProcessMessagesLoop, CancellationToken);
        }
        
        public CancellationToken CancellationToken { get; }

        // TODO: Mouse Interaction https://stackoverflow.com/questions/1944481/console-app-mouse-click-x-y-coordinate-detection-comparison
        public bool IsMouseEnabled
        {
            get => isMouseEnabled;
            set
            {
                if (value)
                {
                    ConsoleInteropHelper.EnableMouseSupport();
                }
                else
                {
                    MousePosition = new VectorInt2(-1);
                }
                isMouseEnabled = value;
            }
        }

        public VectorInt2 MousePosition { get; set; }  = new VectorInt2(-1);
        public bool IsMouseClick => MouseLeftClick > 254;

        public bool IsKeyPressed(ConsoleKey key) => KeyDown[(byte) key] > 254;
        
        public int[] KeyDown  = new int[256];
        public int MouseLeftClick { get; set; }
        
        private bool isMouseEnabled;

        private void ProcessMessagesLoop()
        {
            var inputHandle =  ConsoleInteropHelper.Get_STD_INPUT_HANDLE;
            var inputRecords = Enumerable.Range(0, 10).Select(x => new INPUT_RECORD()).ToArray();

            uint numRead = 0;
            while (!CancellationToken.IsCancellationRequested)
            {
                ConsoleInterop.ReadConsoleInput(inputHandle, inputRecords, (uint)inputRecords.Length, out numRead);
                if (numRead > 0)
                {
                    foreach (var rec in inputRecords)
                    {
                        if (rec.EventType == INPUT_RECORD.MOUSE_EVENT && isMouseEnabled)
                        {
                            MousePosition = new VectorInt2(rec.MouseEvent.dwMousePosition.X, rec.MouseEvent.dwMousePosition.Y);
                            if ((rec.MouseEvent.dwButtonState & 1) > 0)
                            {
                                MouseLeftClick = 255;
                            }
                        }
                        else if (rec.EventType == INPUT_RECORD.KEY_EVENT)
                        {
                            if (rec.KeyEvent.wVirtualKeyCode < KeyDown.Length)
                            {
                                KeyDown[rec.KeyEvent.wVirtualKeyCode] = rec.KeyEvent.bKeyDown ? 255 : 0;
                            }
                        }
                    } 
                }
            }
        }

        


        public void Dispose()
        {
            background.Dispose();
        }

        public void Step()
        {
            for(var i=0; i<KeyDown.Length; i++)
                if (KeyDown[i] > 0)
                    KeyDown[i]--;

            if (MouseLeftClick > 0) MouseLeftClick--;
        }
    }
}