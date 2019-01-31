using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        void Fill(TPixel p);
        TPixel this[int x, int y] { get; set; } // Get/Set Pixel
        TPixel this[float x, float y] { get; set; } // Get/Set Pixel

        void DrawText(int x, int y, string txt, TPixel style);

        void Update();
    }

    public static class RendererExt
    {
        public static bool PixelEquals(float x1, float x2) => System.Math.Abs(x1 - x2) < 0.01;

        

        // https://en.wikipedia.org/wiki/Line_drawing_algorithm
        public static void DrawLine<T>(this IRenderer<T> rr, float x1, float y1, float x2, float y2, T pixel)
        {
            if (x1 > x2)
            {
                var p = x1;
                x1 = x2;
                x2 = p;
            }
            
            if (y1 > y2)
            {
                var p = y1;
                y1 = y2;
                y2 = p;
            }

            if (PixelEquals(x1, x2))  // x1 == x2
            {
                for (var y = y1; y < y2; y++)
                {
                    rr[x1, y] = pixel;
                }
            }
            else
            {
                var dx = x2 - x1;
                var dy = y2 - y1;
                for (var x = x1; x < x2; x++)
                {
                    var y = y1 + dy * (x - x1) / dx;
                    rr[x, y] = pixel;
                }
            }
        }
    }
}
