using ConsoleZ.Core.Buffer;

namespace ConsoleZ.Tests;

public class DrawBoxTests
{
    [Fact]
    public void CanDraw()
    {
        var buf = new ScreenBuffer<int>(10,10);
        buf.Set(0,0, 1,1, 'x');
        buf.Set(9,9, 1,1, 'x');

        Assert.Equal('x', buf[0,0].Chr);
        Assert.Equal('x', buf[9,9].Chr);

        var box = Glyphs.SingleText;
        buf.DrawBox(2,2, box);

        Assert.Equal(box.TopLeft, buf[0,0].Chr);
        Assert.Equal(box.BottomRight, buf[9,9].Chr);
    }
}

