namespace ConsoleZ.Core.Enriched;

public struct EscapedString
{
    public EscapedString(string str, int displayLength)
    {
        String = str;
        DisplayLength = displayLength;
    }

    public string String { get; set; }

    /// <summary>Display  Length without escape chars </summary>
    public int DisplayLength { get; set; }
}

