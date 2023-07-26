namespace NeoLemmixSharp.Engine.Engine.Timer;

public interface ITimer
{
    int ElapsedTicks { get; }

    void Tick();
    ReadOnlySpan<int> AsSpan();
}