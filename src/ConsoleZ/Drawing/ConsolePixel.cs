using System.Drawing;

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

        public char  Char { get; set; }
        public Color Fore { get; set; }
        public Color Back { get; set; }
    }
}