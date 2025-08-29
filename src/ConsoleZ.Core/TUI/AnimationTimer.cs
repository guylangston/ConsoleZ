namespace ConsoleZ.Core.TUI;

public class AnimationTimer
{
    int goal;
    int frames;
    DateTime start;
    double fps;
    private readonly TimeSpan fpsGoal;
    DateTime last;
    TimeSpan lastFrameTime;

    public AnimationTimer(int goal)
    {
        this.goal = goal;
        this.fpsGoal = TimeSpan.FromSeconds(1 / (double)goal);
    }

    public void Start()
    {
        frames = 0;
        last = start = DateTime.Now;
    }

    public int Frames => frames;
    public double FPS => fps;
    public TimeSpan Elapsed => DateTime.Now - start;
    public TimeSpan LastFrameTime => lastFrameTime;
    public TimeSpan NextFrame()
    {
        frames++;
        var diff = Elapsed.TotalSeconds;
        fps = (double)frames / diff;

        lastFrameTime = DateTime.Now - last;
        last = DateTime.Now;
        return lastFrameTime;
    }

    public void WaitIfNeeded(TimeSpan elapsed)
    {
        if (elapsed < fpsGoal)
        {
            var delayNeeded = fpsGoal - elapsed;
            var diffMs = (int)delayNeeded.TotalMilliseconds;
            Thread.Sleep(diffMs);
        }
    }

    /// <return>elapsed ms</return>
    public double WaitIfNeeded(DateTime lastDraw)
    {
        var elapsed = (DateTime.Now - lastDraw);
        WaitIfNeeded(elapsed);
        return elapsed.TotalMilliseconds;
    }
}

