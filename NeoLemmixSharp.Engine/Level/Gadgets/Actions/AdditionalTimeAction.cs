using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public sealed class AdditionalTimeAction : GadgetAction
{
    private readonly int _additionalSeconds;

    public AdditionalTimeAction(int additionalSeconds)
        : base(GadgetActionType.AddLevelTime)
    {
        _additionalSeconds = additionalSeconds;
    }

    public override void PerformAction(Lemming lemming)
    {
        LevelScreen.LevelTimer.AddAdditionalSeconds(_additionalSeconds);
    }
}
