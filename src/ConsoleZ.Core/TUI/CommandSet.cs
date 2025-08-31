namespace ConsoleZ.Core.TUI;

public record Mapping<TInput>(ITextAppCommand Command, TInput Input);

public class CommandSet<TInput>
{
    Dictionary<string, ITextAppCommand> commands = new();
    List<Mapping<TInput>> mappings = new();

    public IReadOnlyDictionary<string, ITextAppCommand> Commands => commands;
    public IReadOnlyList<Mapping<TInput>> Mappings => mappings;

    public ITextAppCommand Register(ITextAppCommand cmd)
    {
        if (commands.ContainsKey(cmd.Name)) throw new Exception($"Name already exists: {cmd.Name}");
        commands.Add(cmd.Name, cmd);
        return cmd;
    }

    public Mapping<TInput> Map( TInput input, ITextAppCommand cmd)
    {
        if (!commands.ContainsKey(cmd.Name)) Register(cmd);
        var map = new Mapping<TInput>(cmd, input);
        mappings.Add(map);
        return map;
    }

    public bool TryFindInput(TInput input, out Mapping<TInput> map)
    {
        foreach(var item in mappings)
        {
            if (item.Input.Equals(input))
            {
                map = item;
                return true;
            }
        }
        map = default;
        return false;
    }
}

public static class CommandFactory
{
    public static ITextAppCommand Create(string name, Action action)
    {
        return new AppCommandFunc(name, name, null, (_, _, _) => action() );
    }

}


