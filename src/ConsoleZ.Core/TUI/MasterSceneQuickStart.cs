using ConsoleZ.Core.Buffer;
using ConsoleZ.Core.Input;

namespace ConsoleZ.Core.TUI;

public abstract class MasterSceneQuickStart<TInput> : MasterSceneApp<ConsoleColor, TInput>
{
    readonly IKeyboardInputComponent<TInput> compInput;

    public MasterSceneQuickStart(IKeyboardInputComponent<TInput> compInput) : base(StyleProviderTemplates.CreateStdConsole())
    {
        this.compInput = compInput;
        if (!compInput.TryParse("Esc", out escape))
        {
            throw new Exception("Cannot lookup ESC key");
        }
    }

    TInput? unhandled = default(TInput);
    TInput escape;
    protected override bool HandleKeyBefore(HandleKey type, TInput key)
    {
        unhandled = default(TInput);
        if (compInput.AreEqual(key, escape))
        {
            Host.RequestQuit();
            return true;
        }
        unhandled = key;
        return false;
    }

    protected override void DrawHeader(IScreenBuffer<ConsoleColor> header)
    {
        header.Fill(ConsoleColor.Black, ConsoleColor.Gray, ' ');
        header.WriteTextOnly(0,0, GetType().Name);

        var right = $"v{Environment.Version} ${Environment.CommandLine}";
        header.WriteTextOnly(header.Width - right.Length - 1, 0, right);
    }

    protected override void DrawFooter(IScreenBuffer<ConsoleColor> footer)
    {
        footer.Fill(ConsoleColor.Black, ConsoleColor.Gray, ' ');
        var right = "TIME: " + DateTime.Now.ToString("u");
        footer.WriteTextOnly(footer.Width - right.Length - 1, 0, right);

        var middle = $"FPS: {Host.Timer.FPS:0.0}";
        footer.WriteTextOnly(footer.Width/2 - middle.Length/2, 0, middle);

        if (unhandled != null)
        {
            footer.WriteTextOnly(0,0, $"Unhandled: {unhandled}");
        }
    }
}

