using System.ComponentModel.Design;

namespace ConsoleZ.Core.TUI;

// Host does not need to know about drawing implementation details like canvus, keys, etc
public interface ITextApplicationHost : IDisposable
{
    string[] Args { get; }
    ITextApplication Implementation { get; }
    AnimationTimer Timer { get; }
    bool IsRunning { get; }
    void Run();
    void RequestQuit();
    void RequestRedraw();
    void SendHost(object message);
}

// Draw does not need to know about actual canvas yet
public interface ITextApplication
{
    ITextApplicationHost Host { get; }
    void Init(ITextApplicationHost host); // prepare resources
    void Step(); // advance logic
    void Draw(); // called by ITextApplicationHost, so should not know about buffers/screens etc
}

public enum HandleKey { Down, Press, Up }
public interface ITextApplicationInput<TKey>
{
    void HandleKey(HandleKey type, TKey key);
}

public interface ITextScene
{
    void Init(ITextApplication app, int width, int height);
    void Step(); // advance local logic (not the whole app)
}
public interface ITextScene<TCanvas> : ITextScene
{
    void Draw(TCanvas canvas);
}

public interface ITextScene<TCanvas, TKey> : ITextScene<TCanvas>, ITextApplicationInput<TKey>
{
}

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
    public abstract void HandleKey(HandleKey type, TKey key);
    public abstract void Step();
}

public interface ICommandContext
{
    ITextApplicationHost Host { get; }
    ITextApplication App { get; }
    ITextScene? Scene { get; }
    IServiceProvider? ServiceProvider { get; }
}

public class CommandContext(ITextApplicationHost host, ITextApplication app, ITextScene? scene, IServiceProvider? serviceProvider) : ICommandContext
{
    public ITextApplicationHost Host { get; } = host;
    public ITextApplication App { get; } = app;
    public ITextScene? Scene { get; } = scene;
    public IServiceProvider? ServiceProvider { get; } = serviceProvider;
}

public interface ICommandArgs
{
    IReadOnlyDictionary<string, object>? NamedArgs { get; }
}

public class CommandArgs(IReadOnlyDictionary<string, object>? namedArgs) : ICommandArgs
{
    public IReadOnlyDictionary<string, object>? NamedArgs { get; } = namedArgs;

    public static readonly CommandArgs Empty = new CommandArgs(null);
}

public interface ITextAppCommand
{
    string Name { get; }
    string Description { get; }
    bool CanExecute(ICommandContext ctx, ICommandArgs args);
    void Execute(ICommandContext ctx, ICommandArgs args);
}
