namespace ConsoleZ.Core.TUI;

public class TextApplicationHost : ITextApplicationHost
{
    AnimationTimer frameTimer;
    bool running = false;
    DrawContext drawContext;
    public string[] Args { get; }
    protected ITextApplication app;

    public TextApplicationHost(string[] args, ITextApplication app, int goalFps = 60)
    {
        this.frameTimer = new AnimationTimer(goalFps);
        Args = args;
        this.app = app;
    }

    public struct DrawContext
    {
        public bool IsFinalFrame { get; set; }
        public Exception? Exception { get; set; }
    }

    protected int Frame => frameTimer.Frames;
    public AnimationTimer Timer => frameTimer;

    public ITextApplication Implementation => app;
    public bool IsRunning => running;

    protected virtual void Init()
    {
        Console.CursorVisible = false;
    }

    public void Run()
    {
        Init();
        app.Init(this);
        running = true;
        frameTimer.Start();

        try
        {
            while (running)
            {
                if (app is ITextApplicationInput<ConsoleKeyInfo> consInfoKey)
                {
                    while(Console.KeyAvailable)
                    {
                        consInfoKey.HandleKey(HandleKey.Press, Console.ReadKey(true));
                    }
                }
                else if (app is ITextApplicationInput<ConsoleKey> consKey)
                {
                    while(Console.KeyAvailable)
                    {
                        consKey.HandleKey(HandleKey.Press, Console.ReadKey(true).Key);
                    }
                }
                app.Step();
                app.Draw();
                frameTimer.NextFrame();
                frameTimer.WaitIfNeeded(frameTimer.LastFrameTime);
            }
            FinalDraw();
        }
        catch(Exception ex)
        {
            DrawError(ex);
        }
        finally
        {
            Console.CursorVisible = true;
        }
    }

    protected virtual void DrawError(Exception ex)
    {
        drawContext = new DrawContext { IsFinalFrame = true, Exception = ex };
        app.Draw();

        throw new Exception("Exception in animation loop", ex);
    }

    public virtual void FinalDraw()
    {
        drawContext = new DrawContext { IsFinalFrame = true };
        app.Draw(); // Final/cleanup draw
    }

    public void Quit() => running = false;

    public void RequestQuit()
    {
        running = false;
    }

    public void RequestRedraw()
    {
        throw new NotSupportedException();
    }

    public void SendHost(object message)
    {
        throw new NotSupportedException();
    }


    public virtual void Dispose()
    {
    }
}
