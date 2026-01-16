using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Global;

public sealed class AdditionalTimeBehaviour : GadgetBehaviour
{
    private readonly int _additionalTimeInSeconds;

    public AdditionalTimeBehaviour(int additionalTimeInSeconds) : base(GadgetBehaviourType.GlobalAdditionalTime)
    {
        _additionalTimeInSeconds = additionalTimeInSeconds;
    }

    protected override void PerformInternalBehaviour()
    {
        LevelScreen.LevelTimer.AddAdditionalSeconds(_additionalTimeInSeconds);
    }
}
