namespace ConsoleZ.Core.TUI;

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



