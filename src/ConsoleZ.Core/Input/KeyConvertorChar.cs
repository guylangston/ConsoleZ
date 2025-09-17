namespace ConsoleZ.Core.Input;

public class KeyConvertorChar : IKeyInputConvertor<char>
{
    public bool TryConvert(char inKey, out KeyInput outKey)
    {
        if (inKey == ' ')
        {
            outKey = KeyInput.FromCannonical(KeyInputCannonical.Space);
            return true;
        }
        if (inKey == '\t')
        {
            outKey = KeyInput.FromCannonical(KeyInputCannonical.Tab);
            return true;
        }
        if (inKey == '\n')
        {
            outKey = KeyInput.FromCannonical(KeyInputCannonical.Enter);
            return true;
        }

        if (inKey == 27)
        {
            outKey = KeyInput.FromCannonical(KeyInputCannonical.Escape);
            return true;
        }

        if (char.IsLetterOrDigit(inKey))
        {
            outKey = KeyInput.FromCannonical(KeyInputCannonical.Create(
                (char.IsUpper(inKey) ? "Shift+" : "") + char.ToUpper(inKey).ToString(),
                inKey));
            return true;
        }

        outKey = default;
        return false;
    }
}
