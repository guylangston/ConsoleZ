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



