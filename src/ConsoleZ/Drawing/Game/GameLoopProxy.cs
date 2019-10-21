using System;

namespace ConsoleZ.Drawing.Game
{
    public abstract class GameLoopProxy : IGameLoop, IDisposable
    {
        protected GameLoopProxy(GameLoopBase parent)
        {
            Parent = parent;
        }

        protected GameLoopBase Parent { get;  }

        public bool IsActive => Parent.IsActive;

        public int FrameCount=> Parent.FrameCount;



        public DateTime EndFrame => Parent.EndFrame;
        public DateTime StartFrame => Parent.StartFrame;
        public DateTime GameStarted => Parent.GameStarted;
        public TimeSpan Elapsed => Parent.Elapsed;




        public float FramesPerSecond => Parent.FramesPerSecond;

        public abstract void Init();

        public virtual void Reset()
        {
            Parent.Reset();
        }
        
        public abstract void Step(float elapsedSec);

        public abstract void Draw();
        
        public abstract void Dispose();
    }
}