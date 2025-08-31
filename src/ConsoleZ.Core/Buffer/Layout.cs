namespace ConsoleZ.Core.Buffer;

public static class Layout
{
    public static IEnumerable<(int idx, IScreenBuffer<TClr> buf)> PartitionIntoColsThenRows<TClr>(IScreenBuffer<TClr> buf, int cols, int rowsPerCol)
    {
        int px = 0, py = 0;
        int cc = 0;
        int cellWidth = buf.Width / cols;
        while(true)
        {
            yield return (cc++, WindowBuffer.FromBuffer(buf, px, py, cellWidth, rowsPerCol));
            px += cellWidth;
            if (px >= buf.Width - cellWidth /* also needs space to write to */)
            {
                py += rowsPerCol;
                px = 0;
            }

            if (py >= buf.Height) yield break;
        }
    }
}
