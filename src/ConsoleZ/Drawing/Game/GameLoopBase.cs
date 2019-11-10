using System;
using System.Threading;
using ConsoleZ.Win32;

namespace ConsoleZ.Drawing.Game
{
    public abstract class GameLoopBase : IGameLoop, IDisposable
    {
        protected GameLoopBase()
        {
            SetGoalFPS(60);
        }

        public bool  IsActive          { get; protected set; }
        public int   FrameCount        { get; protected set; }
        public float Elapsed           { get; protected set; }
        public float FrameIntervalGoal { get; protected set; }
        public float FramesPerSecond   { get; protected set; }
//       
        public void SetDefaultInterval() => SetGoalFPS(60);
        public void SetGoalFPS(int framePerSec) => FrameIntervalGoal = 1f / (float)framePerSec;
        
        public abstract void Init();

        public virtual void Reset()
        {
            
        }

        public abstract void Step(float elapsedSec);

        public abstract void Draw();
        
        public abstract void Dispose();
    }

    public abstract class RenderingGameLoopBase<T> : GameLoopBase, IRenderingGameLoop<T>
    {
        public InputProvider Input { get; }
        public IRenderer<T> Renderer { get; protected set; }

        protected RenderingGameLoopBase(InputProvider inputProvider, IRenderer<T> renderer)
        {
            Input = inputProvider;
            Renderer = renderer;
        }
    }
}