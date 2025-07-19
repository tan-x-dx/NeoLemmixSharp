namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.GeneralBehaviours;

public sealed class AdditionalTimeAction : GeneralBehaviour
{
    private readonly int _additionalSeconds;

    public AdditionalTimeAction(int additionalSeconds)
    {
        _additionalSeconds = additionalSeconds;
    }

    public override void PerformBehaviour()
    {
        LevelScreen.LevelTimer.AddAdditionalSeconds(_additionalSeconds);
    }
}
