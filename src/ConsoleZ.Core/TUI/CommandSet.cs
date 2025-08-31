namespace ConsoleZ.Core.TUI;

public abstract class AppCommand : ITextAppCommand
{
    protected AppCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }
    public string Description { get; }
    public string? Help { get; set; }

    public abstract bool CanExecute(ICommandContext ctx, ICommandArgs args);
    protected abstract void ExecuteImpl(ICommandContext ctx, ICommandArgs args);

    public virtual void Execute(ICommandContext ctx, ICommandArgs args)
    {
        try
        {
            ExecuteImpl(ctx, args);
        }
        catch(Exception ex)
        {
            throw new Exception($"Cannot execute: {Name}/{GetType().Name}", ex);
        }
    }
}

public class AppCommandFunc : AppCommand
{

    public AppCommandFunc(string name, string description,
            Func<ITextAppCommand, ICommandContext, ICommandArgs, bool>? canExecute,
            Action<ITextAppCommand, ICommandContext, ICommandArgs> impl) : base(name, description)
    {
        this.canExecute = canExecute;
        this.impl = impl;
    }

    readonly Func<ITextAppCommand, ICommandContext, ICommandArgs, bool>? canExecute;
    readonly Action<ITextAppCommand, ICommandContext, ICommandArgs> impl;

    public override bool CanExecute(ICommandContext ctx, ICommandArgs args)
    {
        if (canExecute == null) return true;
        return canExecute(this, ctx, args);
    }

    protected override void ExecuteImpl(ICommandContext ctx, ICommandArgs args)
    {
        impl(this, ctx, args);
    }
}

public record Mapping<TInput>(ITextAppCommand Command, TInput Input);

public class CommandSet<TInput>
{
    Dictionary<string, ITextAppCommand> commands = new();
    List<Mapping<TInput>> mappings = new();

    public IReadOnlyDictionary<string, ITextAppCommand> Commands => commands;
    public IReadOnlyList<Mapping<TInput>> Mappings => mappings;

    public ITextAppCommand Register(ITextAppCommand cmd) => commands[cmd.Name] = cmd;

    public Mapping<TInput> Map( TInput input, ITextAppCommand cmd)
    {
        if (!commands.ContainsKey(cmd.Name)) Register(cmd);
        var map = new Mapping<TInput>(cmd, input);
        mappings.Add(map);
        return map;
    }
}

public static class CommandFactory
{
    public static ITextAppCommand Create(string name, Action action)
    {
        return new AppCommandFunc(name, name, null, (_, _, _) => action() );
    }
}


