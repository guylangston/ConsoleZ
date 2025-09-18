namespace ConsoleZ.Core.TUI;

public abstract class TextScene<TCanvas, TKey> : ITextScene<TCanvas, TKey>
{
    private ITextApplication? app;
    protected ITextApplication App => app ?? throw new NullReferenceException("Had Init been called?");
    protected ITextApplicationHost Host => App.Host;
    protected int InitialWidth { get; private set; }
    protected int InitialHeight { get; private set; }

    public virtual void Init(ITextApplication app, int width, int height)
    {
        this.app = app;
        InitialWidth = width;
        InitialHeight = height;
    }

    public abstract void Draw(TCanvas canvas);
    public abstract bool HandleKey(HandleKey type, TKey key); // TODO: Should return bool meaning ishandled
    public abstract void Step();
}

