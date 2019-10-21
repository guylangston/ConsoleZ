using ConsoleZ.Win32;
using VectorInt;

namespace ConsoleZ.Drawing
{
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

        public static void DrawLine<T>(this IRenderer<T> rr, VectorInt2 start, VectorInt2 end, T pixel)
            => rr.DrawLine(start.X, start.Y, end.X, end.Y, pixel);

        public static void Fill<T>(this IRenderer<T> rr, IRectInt rect, T pixel)
        {
            foreach (var p in rect)
            {
                rr[p.X, p.Y] = pixel;
            }
        }

        public static void DrawText<T>(this IRenderer<T> rr, VectorInt2 pos, string text, T style, bool isLeft = true)
        {
            if (isLeft)
            {
                rr.DrawText(pos.X, pos.Y, text, style);
            }
            else
            {
                rr.DrawText(pos.X - text.Length, pos.Y, text, style);
            }
        }

        public static void Box<T>(this IRenderer<T> rr, IRectInt rect, T[] pixel)
        {
            rr.DrawLine(rect.TL, rect.TR, pixel[1]);
            rr[rect.TL] = pixel[0];
            rr[rect.TR] = pixel[2];

            rr.DrawLine(rect.TL, rect.BL, pixel[3]);
            rr.Fill(rect.Inset(1,1,1,1), pixel[4]);
            rr.DrawLine(rect.TR, rect.BR, pixel[5]);
            
            rr.DrawLine(rect.BL, rect.BR, pixel[7]);

            rr[rect.TL] = pixel[0];
            rr[rect.TR] = pixel[2];
            rr[rect.BL] = pixel[6];
            rr[rect.BR] = pixel[8];

        }

        public static CHAR_INFO[] AsciiBox = new[]
        {
            
            new CHAR_INFO(0xda, CHAR_INFO_Attr.FOREGROUND_GRAY),
            new CHAR_INFO(0xc4, CHAR_INFO_Attr.FOREGROUND_GRAY),
            new CHAR_INFO(0xbf, CHAR_INFO_Attr.FOREGROUND_GRAY),
            
            
            new CHAR_INFO(0xb3, CHAR_INFO_Attr.FOREGROUND_GRAY),
            new CHAR_INFO(' ', CHAR_INFO_Attr.FOREGROUND_GRAY),
            new CHAR_INFO(0xb3, CHAR_INFO_Attr.FOREGROUND_GRAY),


            new CHAR_INFO(0xc0, CHAR_INFO_Attr.FOREGROUND_GRAY),
            new CHAR_INFO(0xc4, CHAR_INFO_Attr.FOREGROUND_GRAY),
            new CHAR_INFO(0xd9, CHAR_INFO_Attr.FOREGROUND_GRAY),


        };
    }
}