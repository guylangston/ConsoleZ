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

        public bool IsActive{ get; protected set; }

        public int FrameCount { get; private set; }

        public DateTime EndFrame { get; private set; }
        public DateTime StartFrame { get; private set; }

        public DateTime GameStarted { get; private set; }
        public TimeSpan Elapsed => DateTime.Now - GameStarted;

        public float MinInterval { get; set; } 

        public void SetDefaultInterval() => SetGoalFPS(60);
        public void SetGoalFPS(int framePerSec) => MinInterval = 1f / (float)framePerSec;

        public float FramesPerSecond => (float) FrameCount / (float) Elapsed.TotalSeconds;

        public abstract void Init();

        public virtual void Reset()
        {

        }

        public void Start()
        {
            GameStarted = DateTime.Now;

            IsActive = true;
            while (IsActive)
            {
                StartFrame = DateTime.Now;
                Draw();
                EndFrame = DateTime.Now;
                var elapse = (float)(EndFrame - StartFrame).TotalSeconds;
                if (elapse < MinInterval)
                {
                    Thread.Sleep((int)((MinInterval - elapse)*1000f));
                    elapse = MinInterval;
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