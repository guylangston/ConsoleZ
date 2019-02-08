using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleZ.DisplayComponents
{
    public class ProgressBar : IDisposable
    {
        private IConsole cons;
        private int line;
        private Stopwatch timer;
        private long ticks;
        private long threshold;

        public ProgressBar(IConsole cons, string title)
        {
            this.Title = title;
            this.cons = cons;
            threshold = TimeSpan.FromMilliseconds(300).Ticks;  // roughly 3 times a sec
        }

        public int ItemsDone { get; set; }
        public int ItemsTotal { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public int GraphWidth { get; set; } = 10;

        public TimeSpan Elapsed => timer?.Elapsed ?? default(TimeSpan);

        public double Percentage => timer == null || ItemsTotal == 0 || ItemsDone == 0
            ? 0
            : ItemsDone * 100d / ItemsTotal;

        public double ItemsPerSecond => timer == null || ItemsTotal == 0 || ItemsDone == 0
            ? 0
            : timer.Elapsed.TotalSeconds / ItemsDone;

        public TimeSpan EstimatedDuration => ItemsPerSecond > 0
            ? new TimeSpan()
            : TimeSpan.FromSeconds(ItemsPerSecond * ItemsTotal);


        public ProgressBar Start(int targetCount)
        {
            ItemsTotal = targetCount;

            timer = new Stopwatch();
            timer.Start();
            ticks = timer.ElapsedTicks;

            line = cons.WriteLine(Render());
            
            return this;
        }

        public ProgressBar Increment(string itemCompleteMessage)
        {
            ItemsDone++;

            if ( timer.ElapsedTicks -ticks > threshold)
            {
                ticks = timer.ElapsedTicks;
                Message = itemCompleteMessage;
                Update();
            }
            
            return this;
        }

        public string  Render()
        {
            var a = (int)(Percentage/100d * GraphWidth);
            var b = GraphWidth - a;
            var graph = new string(Ascii.Block100, a) + new string(Ascii.DotMiddle, b);

            var clr = timer == null
                ? "purple"
                : (timer.IsRunning ? "cyan" : "green");
            var r = $"{Percentage,3:0}% {Ascii.BoxVert}^{clr};{graph}^;{Ascii.BoxVert} {ItemsDone,4}/{ItemsTotal} {Title}";
            if (r.Length >= cons.Width)
            {
                return r.Substring(0, cons.Width - 1);
            }
            return r;
        }

        public virtual void Update(bool force = false)
        {
            cons.UpdateLine(line, Render());
        }


        public ProgressBar Stop()
        {
            if (timer.IsRunning)
            {
                timer.Stop();
                Update(true);
            }

            return this;
        }


        public void Dispose()
        {
            if (timer.IsRunning)
            {
                Stop();
            }

        }

        
    }
}
