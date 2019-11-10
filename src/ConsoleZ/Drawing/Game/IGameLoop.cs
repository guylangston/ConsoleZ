using ConsoleZ.Win32;

namespace ConsoleZ.Drawing.Game
{
    public interface IGameLoop
    {
        void Init();
        void Reset();
        void Step(float elapsed);
        void Draw();

        bool  IsActive          { get; }
        int   FrameCount        { get; }
        float Elapsed           { get; }
        float FrameIntervalGoal { get; }
        float FramesPerSecond   { get; }
    }

    public interface IRenderingGameLoop<T> : IGameLoop
    {
        InputProvider Input { get; }
        IRenderer<T> Renderer { get; }
    }
}