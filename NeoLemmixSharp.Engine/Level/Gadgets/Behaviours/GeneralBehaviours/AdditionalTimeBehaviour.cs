namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;

public sealed class AdditionalTimeBehaviour : GadgetBehaviour
{
    private readonly int _additionalSeconds;

    public AdditionalTimeBehaviour(
        int maxTriggerCountPerUpdate,
        int additionalSeconds)
        : base(maxTriggerCountPerUpdate)
    {
        _additionalSeconds = additionalSeconds;
    }

    protected override void PerformInternalBehaviour()
    {
        LevelScreen.LevelTimer.AddAdditionalSeconds(_additionalSeconds);
    }
}
