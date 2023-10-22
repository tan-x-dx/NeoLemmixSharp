using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Skills;

public interface ILemmingStateChanger
{
    void SetLemmingState(LemmingState lemmingState, bool status);
    void ToggleLemmingState(LemmingState lemmingState);

    bool IsApplied(LemmingState lemmingState);
}