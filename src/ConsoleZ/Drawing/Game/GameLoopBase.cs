using System;
using System.Threading;

namespace ConsoleZ.Drawing.Game
{
    public abstract class GameLoopBase : IGameLoop, IDisposable
    {
        protected GameLoopBase()
        {
            SetGoalFPS(60);
        }

        public bool IsActive { get; protected set; }
        public int FrameCount { get; protected set; }
        public DateTime EndFrame { get; protected set; }
        public DateTime StartFrame { get; protected set; }
        public DateTime GameStarted { get; protected set; }
        public TimeSpan Elapsed => DateTime.Now - GameStarted;
        public float MinIntervalSec { get; set; }
        public TimeSpan MinIntervalTimeSpan => TimeSpan.FromSeconds(MinIntervalSec);
        public float FramesPerSecond => (float) FrameCount / (float) Elapsed.TotalSeconds;
        
        public void SetDefaultInterval() => SetGoalFPS(60);
        public void SetGoalFPS(int framePerSec) => MinIntervalSec = 1f / (float)framePerSec;
        

        public abstract void Init();

        public virtual void Reset()
        {

        }

        public virtual void Start()
        {
            GameStarted = DateTime.Now;

            IsActive = true;
            while (IsActive)
            {
                StartFrame = DateTime.Now;
                Draw();
                EndFrame = DateTime.Now;
                var elapse = (float)(EndFrame - StartFrame).TotalSeconds;
                if (elapse < MinIntervalSec)
                {
                    Thread.Sleep((int)((MinIntervalSec - elapse)*1000f));
                    elapse = MinIntervalSec;
                }

                Step(elapse);
                FrameCount++;
            }
            // Dispose should be called next
        }
        

        public abstract void Step(float elapsedSec);

        public abstract void Draw();
        
        public abstract void Dispose();
    }
}