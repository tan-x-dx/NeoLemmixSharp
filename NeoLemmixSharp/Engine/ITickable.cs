namespace NeoLemmixSharp.Engine;

public interface ITickable
{
    bool ShouldTick { get; }

    void Tick();
}