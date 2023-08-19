using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Updates;

public interface IFrameUpdater
{
    UpdateState UpdateState { get; }

    void UpdateLemming(Lemming lemming);
    bool Tick();
}