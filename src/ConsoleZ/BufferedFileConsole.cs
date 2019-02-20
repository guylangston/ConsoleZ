using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleZ
{
    public class BufferedFileConsole : ConsoleBase, IDisposable
    {
        private readonly TextWriter outp;

        public BufferedFileConsole(TextWriter outp,  string handle, int width, int height) : base(handle, width, height)
        {
            this.outp = outp ?? throw new ArgumentNullException(nameof(outp));
        }

        public override void LineChanged(int index, string line, bool updated)
        {
            // Nothing, wait to the end

            // TODO: Every 5min, write everything?
        }


        public void Dispose()
        {
            if (Renderer == null)
            {
                foreach (var line in base.lines)
                {
                    outp.WriteLine(line);
                }
            }
            else
            {
                int cc = 0;
                foreach (var line in base.lines)
                {
                    outp.WriteLine(Renderer.RenderLine(this, cc++, line));
                }
            }

            outp.Dispose();
        }
    }
}
