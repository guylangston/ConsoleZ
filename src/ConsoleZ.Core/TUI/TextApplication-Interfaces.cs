namespace ConsoleZ.Core.TUI;

// Host does not need to know about drawing implementation details like canvus, keys, etc
public interface ITextApplicationHost : IDisposable
{
    string[] Args { get; }
    ITextApplication Implementation { get; }
    AnimationTimer Timer { get; }

    /// <summary>Is the animation loop running</summary>
    bool IsRunning { get; }

    /// <summary>While running, it may be useful to Pause the application to allow normal terminal operation</ry>
    bool IsPaused { get; }
    void RequestPauseAndRun(Action runThenUnPause);

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
    /// <returns>false: unhandled</returns>
    bool HandleKey(HandleKey type, TKey key);
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

public interface ICompoundScene<TCanvas, TKey>
{
    ITextScene<TCanvas, TKey> Current { get; }
    void Push(ITextScene<TCanvas, TKey> newScene);
    void Pop();
}
