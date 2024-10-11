using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class AdditionalTimeAction : IGadgetAction
{
    private readonly int _additionalSeconds;

    public AdditionalTimeAction(int additionalSeconds)
    {
        _additionalSeconds = additionalSeconds;
    }

    public void PerformAction(Lemming lemming)
    {
        LevelScreen.LevelTimer.AddAdditionalSeconds(_additionalSeconds);
    }
}