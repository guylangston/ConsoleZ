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
            this.cons = cons ?? throw new ArgumentNullException(nameof(cons));

            this.Title = title;
            threshold = TimeSpan.FromMilliseconds(300).Ticks;  // roughly 3 times a sec
        }

        public IConsole Console => cons;

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

        public TimeSpan Duration => timer.Elapsed;

        public TimeSpan EstimatedDuration => ItemsDone <= 0
            ? new TimeSpan()
            : TimeSpan.FromSeconds(ItemsPerSecond * ItemsTotal);

        public TimeSpan EstimatedRemaining => ItemsDone <= 0
            ? new TimeSpan()
            : EstimatedDuration - Duration;


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

        public virtual string Render()
        {
            var a = (int)(Percentage/100d * GraphWidth);
            var b = GraphWidth - a;
            var graph = new string(Ascii.Block100, a) + new string(Ascii.DotMiddle, b);

            var clr = timer == null
                ? "purple"
                : (timer.IsRunning ? "cyan" : "green");

            var clr2 = timer == null
                ? "purple"
                : (timer.IsRunning ? "purple" : "yellow");

            string time = null;
            if (timer == null || ItemsDone <= 0)
            {
                time = "Pending";
            }
            else if (timer.IsRunning)
            {
                time = $"{Humanize(EstimatedRemaining)} left";
            }
            else
            {
                time = $"Done in {Humanize(Duration)}";
            }
            
            var r = $"{Percentage,3:0}% {Ascii.BoxVert}^{clr};{graph}^;{Ascii.BoxVert} {ItemsDone,4}/{ItemsTotal,-4} ^{clr2};{time,-15}^; | {Title}";
            if (!string.IsNullOrEmpty(Message))
            {
                r += " > " + Message;
            }
            if (r.Length >= cons.Width)
            {
                return r.Substring(0, cons.Width - 1);
            }
            return r;
        }

        public virtual void Update(bool force = false)
        {
            var txt = Render();
            var u = cons.UpdateLine(line, txt);
            if (force && !u)
            {
                // Could not update, so write a new line
                cons.WriteLine(txt);
            }
        }


        public ProgressBar Stop()
        {
            if (timer.IsRunning)
            {
                timer.Stop();
            }
            Update(true);

            return this;
        }

        public static string Humanize(TimeSpan span)
        {
            if (span.TotalSeconds < 1) return $"{span.Milliseconds} ms";
            if (span.TotalMinutes < 1) return $"{span.Seconds} sec";
            if (span.TotalHours < 1) return $"{span.Minutes} min";
            if (span.TotalDays < 1) return $"{span.Hours} hr, {span.Minutes} min";
            if (span.TotalDays > 365) return $"{(int)span.TotalDays/365} yrs, {(int)span.TotalDays % 365} days"; 

            if (span.Hours == 0) return $"{span.Days} days";
            return $"{span.Days} days, {span.Hours} hr";
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
