namespace ConsoleZ.Core.Input;

public class KeyParser : IKeyInputParser
{
    public static string NativeToCanonical(char native)
    {
        if (char.IsUpper(native)) return $"Shift+{native}";
        if (char.IsLower(native)) return char.ToUpper(native).ToString();

        return native.ToString();
    }

    public bool TryParse(string key, out KeyInput outKey)
    {
        // var sep = key.IndexOf("::");
        // if (sep > 0)
        // {
        //     var source = key[sep..];
        //     if (key.StartsWith(KeySchema.WinForms + "::"))
        //     {
        //         outKey = KeyInput.Create(KeySchema.WinForms, source, source, null);
        //         return true;
        //     }
        //     if (key.StartsWith(KeySchema.Trivial + "::"))
        //     {
        //         outKey = KeyInput.Create(KeySchema.Trivial, key.Remove(0, sep + 2), null);
        //         return true;
        //     }
        //
        //     outKey = default;
        //     return false;
        // }
        //
        if (key.Length == 1)
        {
            // direct match
            if (KeyInput.FromChar(key[0]) is {} keyInput)
            {
                outKey = keyInput;
                return true;
            }
        }
        foreach(var can in KeyInputCannonical.WellKnow)
        {
            if (can.CanonicalString == key)
            {
                outKey = KeyInput.FromCannonical(can);
                return true;
            }
        }

        outKey = KeyInput.None;
        return false;
    }
}
