using System.Collections.Immutable;
using System.Text;

namespace ConsoleZ.Core.Input;

public enum KeySchema
{
    SingleChar,
    WinForms,
    Vim,
    Cannonical,
}

public interface IKeyInputParser
{
    bool TryParse(string txt, out KeyInput key);
}

public interface IKeyInputConvertor<in T>
{
    bool TryConvert(T inKey, out KeyInput outKey);
}

public record class KeyMap(string Cannonical, char? Native, char? NativeShift);

public sealed class KeyInput : KeyInputCannonical
{
    public required KeySchema SourceScheme { get; init; }

    /// <summary>Origonal Input as string</summary>
    public required string Source { get; init; }
    public required IReadOnlyList<string> CanonicalAlt { get; init; }

    public static KeyInput Create(KeySchema schema, string source, string cannonical, char? native)
    {
        return new KeyInput
        {
            SourceScheme = schema,
            Source = source,
            CanonicalString = cannonical,
            NativeChar = native,
            CanonicalAlt =  ImmutableArray<string>.Empty,
        };
    }
    public static KeyInput FromCannonical(KeyInputCannonical cannon)
    {
        return new KeyInput
        {
            SourceScheme = KeySchema.Cannonical,
            Source = cannon.CanonicalString,
            CanonicalString = cannon.CanonicalString,
            NativeChar = cannon.NativeChar,
            CanonicalAlt =  ImmutableArray<string>.Empty,
        };
    }

    public static KeyInput FromChar(char native)
        => Create(KeySchema.SingleChar, native.ToString(), KeyParser.NativeToCanonical(native), native);

    public readonly static KeyInput None = new KeyInput
    {
        SourceScheme = KeySchema.SingleChar,
        Source = "",
        NativeChar = null,
        CanonicalString = "None",
        CanonicalAlt = ImmutableArray<string>.Empty
    };

    public static KeyInput FromConsoleKeyInfo(ConsoleKeyInfo info)
    {
        var txt = new StringBuilder();
        if (info.Modifiers.HasFlag(ConsoleModifiers.Shift))
        {
            txt.Append("Shift+");
        }
        if (info.Modifiers.HasFlag(ConsoleModifiers.Alt))
        {
            txt.Append("Alt+");
        }
        if (info.Modifiers.HasFlag(ConsoleModifiers.Control))
        {
            txt.Append("Ctrl+");
        }
        var toKey =  info.Key switch
        {
            ConsoleKey.UpArrow    => Up.CanonicalString,
            ConsoleKey.DownArrow  => Down.CanonicalString,
            ConsoleKey.LeftArrow  => Left.CanonicalString,
            ConsoleKey.RightArrow => Right.CanonicalString,
            ConsoleKey.Spacebar   => Space.CanonicalString,
            ConsoleKey.Escape     => Escape.CanonicalString,
            ConsoleKey.Enter      => Enter.CanonicalString,
            _ => info.Key.ToString()
        };
        txt.Append(toKey);

        return new KeyInput
        {
            SourceScheme = KeySchema.Cannonical,
            CanonicalString = txt.ToString(),
            CanonicalAlt = [],
            Source = info.Key.ToString(),
            NativeChar = info.KeyChar,
        };
    }

    public override string ToString() => $"<{CanonicalString}>";
    // public override string ToString() => TextHelper.Build()
    //     .Append("<")
    //     .Append(CanonicalString)
    //     .If(CanonicalAlt is { Count: > 0 }, x =>
    //     {
    //         foreach (var alt in CanonicalAlt)
    //         {
    //             x.Append("|");
    //             x.Append(alt);
    //         }
    //     })
    //     .Append(">")
    //     .ToString();
}
