using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VectorInt;

namespace ConsoleZ.Win32
{
    public interface IInputProvider : IDisposable
    {
        bool       IsMouseEnabled { get; set; }
        VectorInt2 MousePosition  { get; set; }
        bool       IsMouseClick   { get; }

        bool IsKeyDown(ConsoleKey key);
        bool IsKeyPressed();
        bool IsKeyPressed(ConsoleKey key);
        void Step(float elapsed);
    }
    
    

    public class InputProvider : IInputProvider
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
        public bool IsMouseClick => MouseLeftClick > 0;

        public bool IsKeyDown(ConsoleKey key) => KeyDown[(byte) key] > 0;
        public bool IsKeyPressed() => KeyDown.Any(x=>x >  0);
        
        public bool IsKeyPressed(ConsoleKey key)
        {
            if (KeyDown[(byte) key] > 0)
            {
                KeyDown[(byte) key] = 0;
                return true;
            }
            return false;
        }

        public float[] KeyDown  = new float[256];
        public float MouseLeftClick { get; set; }
        
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
                                MouseLeftClick = 0.0001f;
                            }
                        }
                        else if (rec.EventType == INPUT_RECORD.KEY_EVENT)
                        {
                            if (rec.KeyEvent.wVirtualKeyCode < KeyDown.Length)
                            {
                                if (rec.KeyEvent.bKeyDown)
                                {
                                    KeyDown[rec.KeyEvent.wVirtualKeyCode] = 0.0001f;
                                }
                                else
                                {
                                    // up
                                    KeyDown[rec.KeyEvent.wVirtualKeyCode] = 0;    
                                }
                                
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

        public void Step(float elapsed)
        {
            for(var i=0; i<KeyDown.Length; i++)
                if (KeyDown[i] > 0)
                    KeyDown[i] += elapsed;

            if (MouseLeftClick > 0) MouseLeftClick+=elapsed;
        }
    }
}