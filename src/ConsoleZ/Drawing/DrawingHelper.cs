using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleZ.Win32;
using VectorInt;

namespace ConsoleZ.Drawing
{
    public static class DrawingHelper
    {
        public static void DrawByteMatrix<TPixel>(IRenderer<TPixel> render, VectorInt2 pos, Func<byte, TPixel> getPixel)
        {
            for (int x = 0; x < 16; x++)
            {
                for(int y=0; y<16; y++)
                {
                    render[pos.X +x , pos.Y + y] = getPixel((byte)(x + (16*y)));
                }
            }
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
