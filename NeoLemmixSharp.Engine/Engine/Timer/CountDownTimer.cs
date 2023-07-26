namespace NeoLemmixSharp.Engine.Engine.Timer;

public sealed class CountDownTimer : ITimer
{
    public int ElapsedTicks { get; private set; }

    public void Tick()
    {

    }

    public ReadOnlySpan<int> AsSpan()
    {
        throw new NotImplementedException();
    }
}