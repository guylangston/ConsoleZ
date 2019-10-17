using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleZ.Win32;
using VectorInt;

namespace ConsoleZ.Drawing
{

    public struct ConsolePixel
    {
        public ConsolePixel(char c, Color fore, Color back)
        {
            Char = c;
            Fore = fore;
            Back = back;
        }

        public char Char { get; set; }
        public Color Fore { get; set; }
        public Color Back { get; set; }
    }

    public interface IRenderer<TPixel>
    {
        int Height { get; }
        int Width { get; }
        
        RectInt Geometry { get; }

        void Fill(TPixel p);
        TPixel this[int x, int y] { get; set; } // Get/Set Pixel
        TPixel this[VectorInt2 p] { get; set; } // Get/Set Pixel
        TPixel this[float x, float y] { get; set; } // Get/Set Pixel

        void DrawText(int x, int y, string txt, TPixel style);

        void Update();
    }

    public  abstract class ConsoleRenderer<TPixel> : IRenderer<TPixel>
    {
        private readonly IBufferedAbsConsole<TPixel> console;

        protected ConsoleRenderer(IBufferedAbsConsole<TPixel> console)
        {
            this.console = console;
            this.Geometry = new RectInt(0, 0, console.Width, console.Height);
        }

        public int Height => console.Height;
        public int Width => console.Width;
        public RectInt Geometry { get; }

        public string Handle => console.Handle;

        public void Fill(TPixel p) => console.Fill(p);

        public TPixel this[int x, int y]
        {
            get => console[x,y];
            set => console[x, y] = value;
        }

        public TPixel this[VectorInt2 p]
        {
            get => console[p.X, p.Y];
            set => console[p.X, p.Y] = value;
        }

        public TPixel this[float x, float y]
        {
            get => console[(int)x, (int)y];
            set => console[(int)x, (int)y] = value;
        }

        public abstract void DrawText(int x, int y, string txt, TPixel style);

        public void Update() => console.Update();
    }

    public class ConsoleRendererCHAR_INFO : ConsoleRenderer<CHAR_INFO>
    {
        public ConsoleRendererCHAR_INFO(IBufferedAbsConsole<CHAR_INFO> bufferedAbsConsole) : base(bufferedAbsConsole)
        {
        }

        public override void DrawText(int x, int y, string txt, CHAR_INFO style)
        {
            var c = x;
            foreach (var chr in txt)
            {
                this[c++, y] = new CHAR_INFO(chr, style.Attributes);
            }
        }
    }
}
