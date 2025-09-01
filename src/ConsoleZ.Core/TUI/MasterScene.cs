using ConsoleZ.Core.Buffer;

namespace ConsoleZ.Core.TUI;

public abstract class MasterSceneApp<TClr, TInput> : TextScene<IScreenBuffer<TClr>, TInput>
{
    protected StyleProvider<TClr> Style { get; set; }

    protected MasterSceneApp(StyleProvider<TClr> style)
    {
        Style = style;
    }

    protected abstract void DrawHeader(IScreenBuffer<TClr> header);
    protected abstract void DrawBody(IScreenBuffer<TClr> body);
    protected abstract void DrawFooter(IScreenBuffer<TClr> footer);

    public bool IsHeaderEnabled { get; set; } = true;
    public bool IsFooterEnabled { get; set; } = true;

    public override void Draw(IScreenBuffer<TClr> canvas)
    {
        var bodyStart = 0;
        var bodyShrink = 0;

        // Header
        if (IsHeaderEnabled)
        {
            var header = WindowBuffer.FromBuffer(canvas, 0, 0, canvas.Width, 1);
            header.Fill(Style.GetFg("Header.Fg"), Style.GetBg("Header.Bg"), ' ');
            DrawHeader(header);
            bodyStart++;
            bodyShrink++;
        }

        // Footer
        if (IsFooterEnabled)
        {
            var footer = WindowBuffer.FromBuffer(canvas, 0, canvas.Height-1, canvas.Width, 1);
            footer.Fill(Style.GetFg("Footer.Fg"), Style.GetBg("Footer.Bg"), ' ');
            DrawFooter(footer);
            bodyShrink++;
        }

        // Body
        var body = WindowBuffer.FromBuffer(canvas, 0, bodyStart, canvas.Width, canvas.Height-bodyShrink);
        body.Fill(Style.GetFg("Body.Fg"), Style.GetBg("Body.Bg"), ' ');
        DrawBody(body);
    }
}

