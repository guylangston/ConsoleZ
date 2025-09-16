using ConsoleZ.Core.Buffer;

namespace ConsoleZ.Core.TUI;

public abstract class MasterSceneQuickStart<TInput> : MasterSceneApp<ConsoleColor, TInput>
{
    readonly IInputComponent<TInput> compInput;

    public MasterSceneQuickStart(IInputComponent<TInput> compInput) : base(StyleProviderTemplates.CreateStdConsole())
    {
        this.compInput = compInput;
    }

    TInput? unhandled = default(TInput);
    public override bool HandleKey(HandleKey type, TInput key)
    {
        unhandled = default(TInput);
        if (compInput.AreEqual(key, 'q'))
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
    }

    protected override void DrawFooter(IScreenBuffer<ConsoleColor> footer)
    {
        footer.Fill(ConsoleColor.Black, ConsoleColor.Gray, ' ');
        var right = "TIME: " + DateTime.Now.ToString("z");

        footer.WriteTextOnly(footer.Width - right.Length - 1, 0, right);

        if (unhandled != null)
        {
            footer.WriteTextOnly(0,0, $"Unhandled: {unhandled}");
        }
    }

}


