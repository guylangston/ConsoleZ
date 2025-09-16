using System.Diagnostics.CodeAnalysis;
using ConsoleZ.Core.Buffer;

namespace ConsoleZ.Core.TUI;

public readonly struct TextClr<TClr>(TClr fg, TClr bg)
{
    public TClr Fg { get; } = fg;
    public TClr Bg { get; } = bg;

    public ScreenBuffer<TClr> CreateBuffer(ReadOnlySpan<char> txt)
    {
        return ScreenBuffer<TClr>.FromText(Fg, Bg, txt);
    }

    public ScreenBuffer<TClr> CreateBuffer(int width, int height)
    {
        var buf = new ScreenBuffer<TClr>(width, height);
        buf.Fill(Fg, Bg, ' ');
        return buf;
    }
    public static implicit operator (TClr Fg, TClr Bg)(TextClr<TClr> data) => (data.Fg, data.Bg);

    public void Deconstruct(out TClr fg, out TClr bg)
    {
        fg = this.Fg;
        bg = this.Bg;
    }
}

public interface IStyleProvider<TClr>
{
    TClr DefaultFore { get; }
    TClr DefaultBack { get; }
    bool TryGetColour(string styleName, [NotNullWhen(true)] out TClr? clr);
    bool TryParse(string clrTxt, [NotNullWhen(true)] out TClr? clr);

    TClr GetFg(string styleName);
    TClr GetBg(string styleName);
}

public interface IStyleProviderStd<TClr> : IStyleProvider<TClr>
{
    IReadOnlyList<TClr> Palette { get; }
    TClr A1 { get; }
    TClr A2 { get; }
    TClr A3 { get; }
    TClr A4 { get; }
    TClr A5 { get; }
    TClr A6 { get; }
    TClr A7 { get; }
    TClr A8 { get; }

    TClr B1 { get; }
    TClr B2 { get; }
    TClr B3 { get; }
    TClr B4 { get; }
    TClr B5 { get; }
    TClr B6 { get; }
    TClr B7 { get; }
    TClr B8 { get; }
}

public class StyleProvider<TClr> : IStyleProvider<TClr>
{
    public StyleProvider(Dictionary<string, TClr> colours, TClr defaultFore, TClr defaultBack)
    {
        StyleToColour = colours;
        DefaultFore = defaultFore;
        DefaultBack = defaultBack;
    }

    public StyleProvider(StyleProvider<TClr> copy) : this(new Dictionary<string, TClr>(copy.StyleToColour), copy.DefaultFore, copy.DefaultBack)
    {
    }

    public Dictionary<string, TClr> StyleToColour { get; init; }
    public TClr DefaultFore { get; set; }
    public TClr DefaultBack { get; set; }

    public virtual  bool TryGetColour(string styleName, [NotNullWhen(true)] out TClr? clr)
    {
        if (StyleToColour.TryGetValue(styleName, out var hit))
        {
            if (hit != null)
            {
                clr = hit;
                return true;
            }
        }

        clr = default;
        return false;
    }

    public bool TryParse(string clrTxt, [NotNullWhen(true)] out TClr? clr)
    {
        throw new NotImplementedException();
    }


    /// <summary>Will load a `.Fg` and `.Bg` pair </summary>
    public TextClr<TClr> GetTextStyle(string prefix) => new TextClr<TClr>(GetFg(prefix + ".Fg"), GetBg(prefix + ".Bg"));

    public TClr GetFg(string styleName)
    {
        if (TryGetColour(styleName, out var clr))
        {
            return clr;
        }
        return DefaultFore;
    }
    public TClr GetBg(string styleName)
    {
        if (TryGetColour(styleName, out var clr))
        {
            return clr;
        }
        return DefaultBack;
    }
}

public class StyleProviderStd<TClr> : StyleProvider<TClr>, IStyleProviderStd<TClr>
{
    public StyleProviderStd(IReadOnlyList<TClr> palette, Dictionary<string, TClr> colours, TClr defaultFore, TClr defaultBack) : base(colours, defaultFore, defaultBack)
    {
        Palette = palette;
        StyleToColour[nameof(A1)] = A1;
        StyleToColour[nameof(A2)] = A2;
        StyleToColour[nameof(A3)] = A3;
        StyleToColour[nameof(A4)] = A4;
        StyleToColour[nameof(A5)] = A5;
        StyleToColour[nameof(A6)] = A6;
        StyleToColour[nameof(A7)] = A7;
        StyleToColour[nameof(A8)] = A8;
        StyleToColour[nameof(B1)] = B1;
        StyleToColour[nameof(B2)] = B2;
        StyleToColour[nameof(B3)] = B3;
        StyleToColour[nameof(B4)] = B4;
        StyleToColour[nameof(B5)] = B5;
        StyleToColour[nameof(B6)] = B6;
        StyleToColour[nameof(B7)] = B7;
        StyleToColour[nameof(B8)] = B8;
    }

    public StyleProviderStd(StyleProviderStd<TClr> copy) : this(copy.Palette, new Dictionary<string, TClr>(copy.StyleToColour), copy.DefaultFore, copy.DefaultBack)
    {
    }

    public IReadOnlyList<TClr> Palette { get; init; }

    public TClr A1 => Palette[0];
    public TClr A2 => Palette[1];
    public TClr A3 => Palette[2];
    public TClr A4 => Palette[3];
    public TClr A5 => Palette[4];
    public TClr A6 => Palette[5];
    public TClr A7 => Palette[6];
    public TClr A8 => Palette[7];
    public TClr B1 => Palette[8];
    public TClr B2 => Palette[9];
    public TClr B3 => Palette[10];
    public TClr B4 => Palette[11];
    public TClr B5 => Palette[12];
    public TClr B6 => Palette[13];
    public TClr B7 => Palette[14];
    public TClr B8 => Palette[15];
}

public static class StyleProviderTemplates
{

    public static StyleProviderStd<ConsoleColor> CreateStdConsole()
    {
        var dict = new Dictionary<string, ConsoleColor>();
        var list = new List<ConsoleColor>();
        for(int cc=0; cc<16; cc++)
        {
            var clr = (ConsoleColor)cc;
            list.Add(clr);
            dict[clr.ToString()] = clr;
        }

        return new StyleProviderStd<ConsoleColor>(list, dict, ConsoleColor.Gray, ConsoleColor.Black);
    }
}


