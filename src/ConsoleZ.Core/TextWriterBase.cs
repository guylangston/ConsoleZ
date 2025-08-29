namespace ConsoleZ.Core;

public abstract class TextWriterBase : ITextWriter
{
    public IObjectFormatter? ObjectFormatter { get; set; }

    public abstract void Write(char chr);
    public abstract void Write(ReadOnlySpan<char> s);
    public abstract void WriteLine();

    public virtual void Write(string s) => Write(s.AsSpan());

    public virtual void Write<T>(T? obj)
    {
        if (obj != null)
        {
            if (ObjectFormatter != null && ObjectFormatter.CanFormat(typeof(T), obj))
            {
                ObjectFormatter.WriteTo(this, typeof(T), obj);
                return;
            }
            if (obj.ToString() is {} txt )
            {
                Write(txt);
            }
        }
    }

    public virtual void WriteLine(ReadOnlySpan<char> s)
    {
        Write(s);
        WriteLine();
    }
    public virtual void WriteLine(string s)
    {
        Write(s);
        WriteLine();
    }
    public virtual void WriteLine<T>(T? obj)
    {
        Write(obj);
        WriteLine();
    }
}

