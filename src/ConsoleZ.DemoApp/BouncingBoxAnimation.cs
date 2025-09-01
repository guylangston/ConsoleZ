namespace ConsoleZ.Core.DemoApp;

class BouncingBoxScene : ITextScene<IScreenBuffer<ConsoleColor>, ConsoleKeyInfo>
{
    List<BouncingBox> boxes = new List<BouncingBox>();
    int width, height;
    ITextApplication? app;

    AnimationTimer Timer => app.Host.Timer;

    public void Init(ITextApplication app, int width, int height)
    {
        this.app = app;
        this.width = width;
        this.height = height;

        // define buffer
        var rand = new Random();
        // init list of boxes with random  start positions
        Glyphs.BoxChar[] boxType =  [Glyphs.Single, Glyphs.Double, Glyphs.SingleText];
        for(var i=0; i<5; i++)
        {
            boxes.Add(new BouncingBox()
            {
                x       = rand.Next(0, width-3),
                y       = rand.Next(0, height-3),
                dx      = rand.Next(0,2) == 0 ? 1 : -1,
                dy      = rand.Next(0,2) == 0 ? 1 : -1,
                fg      = (ConsoleColor)rand.Next(1,16),
                bg      = (ConsoleColor)rand.Next(0,16),
                boxchar = boxType[rand.Next(0, boxType.Length)],
            });
        }
    }

    public void Step()
    {
        foreach(var box in boxes)
        {
            box.Step(width, height);
        }
    }

    public void HandleKey(HandleKey type, ConsoleKeyInfo key)
    {
        var kkey = key.Key;
        if (kkey == ConsoleKey.Q) app.Host.RequestQuit();
        if (kkey == ConsoleKey.Escape) app.Host.RequestQuit();
    }

    Queue<double> lastDraws = new ();
    public void Draw(IScreenBuffer<ConsoleColor> buffer)
    {
        lastDraws.Enqueue(Timer.LastFrameTime.TotalMilliseconds);
        while(lastDraws.Count > 10)
            lastDraws.Dequeue();
        ScreenBufferHelper.DrawBox(buffer, ConsoleColor.DarkBlue, ConsoleColor.Black, Glyphs.Single);

        foreach(var box in boxes)
        {
            box.Paint(buffer);
        }

        buffer.Write(3, 0, ConsoleColor.Yellow, ConsoleColor.DarkBlue,
                $"[ Frames: {Timer.Frames} at {Timer.FPS:0.00}f/s, draw time {lastDraws.Average():0.0}ms after {Timer.Elapsed} ]", false);
    }


    class BouncingBox
    {
        public int x, y;
        public int dx, dy;
        public ConsoleColor fg;
        public ConsoleColor bg;
        public Glyphs.BoxChar boxchar;

        public void Paint(IScreenBuffer<ConsoleColor> buf)
        {
            buf.Set(x,   y, fg, bg, boxchar.TopLeft);
            buf.Set(x+1, y, fg, bg, boxchar.TopMiddle);
            buf.Set(x+2, y, fg, bg, boxchar.TopRight);

            buf.Set(x,   y+1, fg, bg, boxchar.MiddleLeft);
            buf.Set(x+1, y+1, fg, bg, boxchar.Middle);
            buf.Set(x+2, y+1, fg, bg, boxchar.MiddleRight);

            buf.Set(x,   y+2, fg, bg, boxchar.BottomLeft);
            buf.Set(x+1, y+2, fg, bg, boxchar.BottomMiddle);
            buf.Set(x+2, y+2, fg, bg, boxchar.BottomRight);
        }

        public void Step( int width, int height)
        {
            x += dx;
            y += dy;
            if (x < 0) { x = 0; dx = -dx;  }
            if (y < 0) { y = 0; dy = -dy;  }
            if (x > width - 3) { x = width - 3; dx = -dx;  }
            if (y > height - 3) { y = height - 3; dy = -dy;  }
        }
    }

}

