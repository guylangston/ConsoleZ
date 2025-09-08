using System.Runtime.Intrinsics.X86;
using ConsoleZ.Core.Buffer;

namespace ConsoleZ.Core.TUI;

public abstract class MasterSceneApp<TClr, TInput> : TextScene<IScreenBuffer<TClr>, TInput>
{
    protected StyleProvider<TClr> Style { get; set; }
    protected CommandSet<TInput> Commands { get; } = new();  // TODO: Only expose readonly interface

    protected MasterSceneApp(StyleProvider<TClr> style)
    {
        Style = style;
    }

    public override void Init(ITextApplication app, int width, int height)
    {
        base.Init(app, width, height);
        InitCommands(Commands);
    }

    protected abstract void InitCommands(CommandSet<TInput> commands);
    protected abstract void DrawHeader(IScreenBuffer<TClr> header);
    protected abstract void DrawBody(IScreenBuffer<TClr> body);
    protected abstract void DrawFooter(IScreenBuffer<TClr> footer);

    public virtual bool IsHeaderEnabled { get; set; } = true;
    public virtual bool IsFooterEnabled { get; set; } = true;

    public override void Draw(IScreenBuffer<TClr> canvas)
    {
        var bodyStart = 0;
        var bodyShrink = 0;

        // Header
        if (IsHeaderEnabled)
        {
            var header = WindowBuffer.FromBuffer(canvas, 0, 0, canvas.Width, 1);
            header.Fill(Style.GetTextStyle("Header"));
            DrawHeader(header);
            bodyStart++;
            bodyShrink++;
        }

        // Footer
        if (IsFooterEnabled)
        {
            var footer = WindowBuffer.FromBuffer(canvas, 0, canvas.Height-1, canvas.Width, 1);
            footer.Fill(Style.GetTextStyle("Footer"));
            DrawFooter(footer);
            bodyShrink++;
        }

        // Body
        var body = WindowBuffer.FromBuffer(canvas, 0, bodyStart, canvas.Width, canvas.Height-bodyShrink);
        body.Fill(Style.GetTextStyle("Body"));
        DrawBody(body);
    }
}

