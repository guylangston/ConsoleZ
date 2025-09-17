using System.Diagnostics.CodeAnalysis;

namespace ConsoleZ.Core.Input;

/// <summary>A genera abstraction to allow keyboard input without knowing the actual scheme used</summary>
public interface IKeyboardInputComponent<TKey>
{
    bool AreEqual(TKey key, char simpleChar);
    bool AreEqual(TKey a, TKey b);
    bool TryParse(string fromStr, [NotNullWhen(true)] out TKey success);
}

public class KeyboardInputComponent : IKeyboardInputComponent<KeyInput>
{
    readonly KeyParser parser = new();

    public bool AreEqual(KeyInput key, char simpleChar) => key.NativeChar == simpleChar;
    public bool AreEqual(KeyInput a, KeyInput b) => a.Equals(b);
    public bool TryParse(string fromStr, [NotNullWhen(true)] out KeyInput success) => parser.TryParse(fromStr, out success);
}


