namespace ConsoleZ.Core.Input;

public class KeyInputCannonical : IEquatable<KeyInputCannonical>
{
    /// <summary> As displayed in unicode </summary>
    public required char? NativeChar { get;  init;}

    /// <summary> "Shift+X", "X", "Plus", "Shift+Plus", "CR", "Space", "Tab" </summary>
    public required string CanonicalString { get; init; }

    public static KeyInputCannonical Create(string cannonical, char? native) => new KeyInputCannonical
    {
        CanonicalString = cannonical,
        NativeChar = native,
    };

    public bool IsSpace()       => Equals(Space);
    public bool IsEscape()      => Equals(Escape);
    public bool IsEnter()       => Equals(Enter);
    public bool IsTab()         => Equals(Tab);
    public bool IsBackspace()   => Equals(Backspace);
    public bool IsDelete()      => Equals(Delete);
    public bool IsUp()          => Equals(Up);
    public bool IsDown()        => Equals(Down);
    public bool IsLeft()        => Equals(Left);
    public bool IsRight()       => Equals(Right);
    public bool IsHome()        => Equals(Home);
    public bool IsEnd()         => Equals(End);

    public readonly static KeyInputCannonical Space     = Create("Space",  ' ');
    public readonly static KeyInputCannonical Escape    = Create("Esc",    null);
    public readonly static KeyInputCannonical Enter     = Create("CR",     '\n');
    public readonly static KeyInputCannonical Tab       = Create("Tab",    '\t');
    public readonly static KeyInputCannonical Backspace = Create("Backspace",   null);
    public readonly static KeyInputCannonical Delete    = Create("Delete", null);
    public readonly static KeyInputCannonical Up        = Create("Up",     null);
    public readonly static KeyInputCannonical Down      = Create("Down",   null);
    public readonly static KeyInputCannonical Left      = Create("Left",   null);
    public readonly static KeyInputCannonical Right     = Create("Right",  null);
    public readonly static KeyInputCannonical Home      = Create("Home",   null);
    public readonly static KeyInputCannonical End       = Create("End",  null);

    public readonly static KeyInputCannonical[] WellKnow =
        [ Space, Escape, Enter, Tab, Backspace, Delete, Up, Down, Left, Right ];

    public bool Equals(KeyInputCannonical? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (CanonicalString == other.CanonicalString) return true;
        if (NativeChar != null && NativeChar == other?.NativeChar) return true;
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is KeyInputCannonical other) return Equals(other);
        throw new NotSupportedException(obj.GetType().Name);
    }

    public override int GetHashCode() => CanonicalString.GetHashCode();

    public override string ToString() => CanonicalString;
}

