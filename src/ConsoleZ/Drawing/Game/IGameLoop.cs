namespace ConsoleZ.Drawing.Game
{
    public interface IGameLoop
    {
        bool IsActive { get; }
        void Init();
        void Step(float elapsed);
        void Draw();
    }
}