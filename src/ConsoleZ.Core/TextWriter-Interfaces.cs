namespace ConsoleZ.Core;

public interface ITextWriterMinimal
{
    void Write(ReadOnlySpan<char> s);
    void WriteLine();
}
public interface ITextWriter  : ITextWriterMinimal  // stop clashes with generic calls
{
    new void Write(ReadOnlySpan<char> s);  // Dup for base to avoid overload clash with Write<T>(T? obj)

    void Write(char chr);
    void Write(string s);
    void Write<T>(T? obj);

    void WriteLine(ReadOnlySpan<char> s);
    void WriteLine(string s);
    void WriteLine<T>(T? obj);
}

public interface IObjectFormatter
{
    bool CanFormat(Type type, object? obj);
    void WriteTo(ITextWriterMinimal outp, Type type, object? obj);
}

// Easy to use but often awkward to implement in an existing hierarchy/type system.
public interface ITextWriterFluent<TSelf> where TSelf:ITextWriterFluent<TSelf>
{
    TSelf Write(char chr);
    TSelf Write(string s);
    TSelf Write(ReadOnlySpan<char> s);
    TSelf Write<T>(T? obj);

    TSelf WriteLine();
    TSelf WriteLine(ReadOnlySpan<char> s);
    TSelf WriteLine(string s);
    TSelf WriteLine<T>(T? obj);
}


