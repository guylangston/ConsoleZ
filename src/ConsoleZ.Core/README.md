# Helpers

## Goals
- Zero redundancy library
- NOTE: The lib takes a dependancy on `MarkDig` -- this is wrong and should be
moved to seperate project.

## Key Classes
- `FluentString` an ease of use wrapper alternative to `StringBuilder`
- `ITextWriter` as `TextWriter` does have a set of interfaces, this cut-down
    type allows TextWriter-like functionality to be added on a variety of different
    types.
- `RichWriter` an `TextWriter`-like interface which adds Fore/Background colour
    and style. A building block for TUIs.
- `DynamicConsole` section of n-lines of the terminal to use as a TUI. With
    Save/Restore functionality.
- `EnrichedStrings` allow terminal string encoding for colours, control, etc.

## TODO
- Move `ITextWriter`,`DynamicConsole`,`EnrichedStrings` into a new
project/package something like 
    - `ConsoleZ.Text`
    - `ConsoleZ.Rendering`
    - `ConsoleZ.Models`
    - `ConsoleZ.App`

# vim: set textwidth=80
