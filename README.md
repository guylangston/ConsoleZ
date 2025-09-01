# ConsoleZ - Create multi-platform Text-based UI

> The Lowest-Common-Demoninator --- build text applications for multiple hosts: Console, TUI, GUI, Web

# PROJECT STATUS : UNSTABLE

Subject to interface changes

## Key Types

- `IScreenBuffer<TClr>` general-purpose text window buffer
- `ITextWriter` general-purpose interface to can be retrofitted to existing classes unlike `System.Text.TextWriter`
- `ITextWriterFluent` a fluent chain-able version of `ITextWriter`
- `Glyphs` special unicode console chars

