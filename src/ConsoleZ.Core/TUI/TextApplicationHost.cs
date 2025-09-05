namespace ConsoleZ.Core.TUI;

public class TextApplicationHost<TInput> : TextApplicationHost
{
    readonly Func<ConsoleKeyInfo, TInput?> toInput;
    readonly ITextApplicationInput<TInput> handleInput;

    public TextApplicationHost(string[] args, ITextApplication app, Func<ConsoleKeyInfo, TInput?> toInput, int goalFps = 60) : base(args, app, goalFps)
    {
        this.toInput = toInput;
        if (app is ITextApplicationInput<TInput> match)
        {
            handleInput = match;
        }
        else
        {
            throw new InvalidCastException("app must be ITextApplicationInput<TInput>");
        }
    }

    protected override void HandleKeyInput()
    {
        while(Console.KeyAvailable)
        {
            var inp = toInput(Console.ReadKey(true));
            if (inp != null)
            {
                handleInput.HandleKey(HandleKey.Press, inp);
            }
            else
            {
                // TODO: Logger?
            }
        }
    }

}

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

    public DrawContext HostDrawContext => drawContext;

    protected int Frame => frameTimer.Frames;
    public AnimationTimer Timer => frameTimer;

    public ITextApplication Implementation => app;
    public bool IsRunning => running;

    protected virtual void Init()
    {
        Console.CursorVisible = false;
    }

    protected virtual void HandleKeyInput()
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
        else
        {
            throw new NotSupportedException();
        }
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
                HandleKeyInput();
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
            RunCompleted();
        }
    }

    protected virtual void RunCompleted()
    {
        Console.CursorVisible = true;
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
