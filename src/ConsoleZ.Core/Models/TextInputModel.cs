using System.Text;
using ConsoleZ.Core.Input;

namespace ConsoleZ.Core.Models;

public class TextInputModel
{
    int cursor;
    TextRange selection;
    readonly List<char> text = new();

    public TextInputModel()
    {
        selection = TextRange.CreateEmpty();
    }

    public TextInputModel(string txt, int viewSize, int viewOffset) : this()
    {
        ViewSize = viewSize;
        ViewOffset = viewOffset;
        foreach(var x in txt) text.Add(x);
        Cursor = Text.Count;
    }

    public IReadOnlyList<char> Text  => text;
    public int Length => Text.Count;
    public int ViewSize { get; set; }      // window size width
    public int ViewOffset { get; set; }    // offset in Text for start of view
    public TextRange Selection => selection;
    public void SetSelection(int start, int end) => selection = new(start, end);

    /// <summary>Cursor Index relative to start (not rel View -- ViewOffset)</summary>
    public int Cursor
    {
        get => cursor;
        set { cursor = value; ConfirmCursor(); }
    }

    public void ConfirmCursor()
    {
        // Don't go beyond the last char (+1)
        if (cursor > text.Count) cursor = text.Count;

        // Keep the cursor in the viewport
        if (cursor >= ViewOffset + ViewSize - 1)
        {
            ViewOffset = cursor - ViewSize + 1;
        }
        else if (cursor < ViewOffset)
        {
            ViewOffset = cursor;
        }
    }

    public record struct ViewPortCell(int Idx, int ViewIdx, char Char, bool IsCursor, bool IsInSelection);

    public IEnumerable<ViewPortCell> GetViewPort()
    {
        for (var cc=0; cc<ViewSize; cc++)
        {
            var idx = ViewOffset + cc;
            var isCursor = idx == Cursor;
            var chr = idx < Text.Count ? Text[idx] : ' ';
            var inSel = selection.Contains(idx);
            yield return new(idx, cc, chr, isCursor, inSel);
        }
    }

    public string GetText() => new string(Text.ToArray());

    public string GetSelectedText()
    {
        if (selection.IsEmpty) return "";
        var sb = new StringBuilder(selection.Length);
        foreach(var i in selection)
            sb.Append(Text[i]);
        return sb.ToString();
    }

    public virtual bool TryHandleInput(KeyInput input)
    {
        if (input.IsLeft())      { MoveLeft(); return true; }
        if (input.IsRight())     { MoveRight(); return true; }
        if (input.IsHome())      { MoveStart(); return true; }
        if (input.IsEnd())       { MoveEnd(); return true; }
        if (input.IsBackspace()) { Backspace(); return true; }
        if (input.IsDelete())    { Delete(); return true; }

        if (input.CanonicalString == "Ctrl+W")     { DeleteWord(); return true; }
        if (input.CanonicalString == "Ctrl+A")     { SelectAll(); return true; }
        if (input.CanonicalString == "Shift+Right"){ IncreaseSelectionRight(); return true; }
        if (input.CanonicalString == "Shift+Left"){ IncreaseSelectionLeft(); return true; }
        if (input.CanonicalString == "Ctrl+Right") { MoveWordRight(); return true; }
        if (input.CanonicalString == "Ctrl+Left")  { MoveWordLeft(); return true; }

        if (input.NativeChar != null && char.IsAscii(input.NativeChar.Value) && (int)input.NativeChar >= 32 )
        {
            AppendAtCursor(input.NativeChar.Value);
            return true;
        }
        return false;
    }

    public void IncreaseSelectionLeft() { throw new NotImplementedException(); }
    public void IncreaseSelectionRight() { throw new NotImplementedException(); }

    public bool MoveWordLeft()  { throw new NotImplementedException(); }
    public bool MoveWordRight() { throw new NotImplementedException(); }
    public virtual void Copy()  { throw new NotSupportedException(); }
    public virtual void Paste() { throw new NotSupportedException(); }
    public void DeleteWord()    { throw new NotImplementedException(); }
    public void DeleteAll()     { throw new NotImplementedException(); }

    public void SelectAll() => selection = new TextRange(0, text.Count - 1);

    public bool MoveLeft()
    {
        if (Cursor > 0)
        {
            Cursor--;
            return true;
        }
        return false;
    }

    public bool MoveRight()
    {
        if (Cursor < Text.Count)
        {
            Cursor++;
            return true;
        }
        return false;
    }

    public bool MoveStart() => Move(0);

    public bool MoveEnd() => Move(Text.Count);

    public bool Move(int pos)
    {
        if (pos <= Text.Count)
        {
            Cursor = pos;
            return true;
        }
        return false;
    }

    public bool Backspace()
    {
        if (!selection.IsEmpty)
        {
            text.RemoveRange(selection.Start, selection.Length);
            selection.SetEmpty();
            ConfirmCursor();
            return true;
        }

        if (Cursor > 0 && Cursor <= Text.Count )
        {
            text.RemoveAt(Cursor-1);
            Cursor--;
            return true;
        }
        return false;
    }

    public bool Delete()
    {
        if (!selection.IsEmpty)
        {
            text.RemoveRange(selection.Start, selection.Length);
            selection.SetEmpty();
            ConfirmCursor();
            return true;
        }
        if (Cursor < Text.Count)
        {
            text.RemoveAt(Cursor);
            return true;
        }
        return false;
    }

    public bool AppendAtCursor(char c)
    {
        if (!selection.IsEmpty)
        {
            // Delete selection
            var s = selection.Start;
            foreach(var _ in selection)
            {
               text.RemoveAt(s);
            }

            // Move cursor to start of selection
            Cursor = s;
        }

        if (Cursor < Text.Count) text.Insert(Cursor, c);
        else text.Add(c);

        Cursor++;
        return true;
    }
}

