using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Updates;

public interface IFrameUpdater
{
    UpdateState UpdateState { get; }

    void UpdateLemming(Lemming lemming);
    bool Tick();
}