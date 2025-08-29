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

public struct DrawContext
{
    public bool IsFinalFrame { get; set; }
    public Exception? Exception { get; set; }
}

// Draw does not need to know about actual canvas yet
public interface ITextApplication
{
    ITextApplicationHost Host { get; }
    void Init(ITextApplicationHost host); // prepare resources
    void Step(); // advance logic
    void Draw(); // called by ITextApplicationHost, so should not know about buffers/screens setc
}

public enum HandleKey { Down, Press, Up }
public interface ITextApplicationInput<TKey>
{
    void HandleKey(HandleKey type, TKey key);
}

public interface ITextScene<TCanvas>
{
    void Init(ITextApplication app, int width, int height);
    void Step(); // advance local logic (not the whole app)
    void Draw(TCanvas canvas);
}

public interface ITextScene<TCanvas, TKey> : ITextScene<TCanvas>, ITextApplicationInput<TKey>
{
}

// TODO
// Keep a queue of key events and allow query for keys, combos (concurrent presses), coords (presses in sequence within some timeframe)
// public interface ITextInputAbstraction
// {
//
// }
