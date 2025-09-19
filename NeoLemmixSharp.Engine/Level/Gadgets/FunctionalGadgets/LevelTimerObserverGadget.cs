using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class LevelTimerObserverGadget : GadgetBase
{
    public LevelTimerObserverGadget()
        : base(GadgetType.LevelTimerObserver)
    {
    }

    public override GadgetState CurrentState { get; }

    public override void Tick()
    {
        CurrentState.Tick();
    }
}

public sealed class LevelTimerTrigger : GadgetTrigger
{
    private readonly OutputSignalBehaviour _outputSignalBehaviour;
    private readonly LevelTimerTriggerParameters _levelTimerTriggerParameters;

    public LevelTimerTrigger(
        OutputSignalBehaviour outputSignalBehaviour,
        LevelTimerTriggerParameters levelTimerTriggerParameters)
        : base(GadgetTriggerType.GlobalLevelTimerTrigger)
    {
        _outputSignalBehaviour = outputSignalBehaviour;
        _levelTimerTriggerParameters = levelTimerTriggerParameters;
    }

    public override void DetectTrigger()
    {
        MarkAsEvaluated();
        if (LevelTimerMatchesParameters())
        {
            DetermineTrigger(true);
            LevelScreen.GadgetManager.RegisterCauseAndEffectData(new CauseAndEffectData(_outputSignalBehaviour.Id, 1));
        }
        else
        {
            DetermineTrigger(false);
        }
    }

    public bool LevelTimerMatchesParameters()
    {
        var comparisonValue = _levelTimerTriggerParameters.ObservationType == LevelTimerObservationType.SecondsElapsed
            ? LevelScreen.LevelTimer.EffectiveElapsedSeconds
            : LevelScreen.LevelTimer.EffectiveSecondsRemaining;

        return _levelTimerTriggerParameters.ComparisonType.ComparisonMatches(comparisonValue, _levelTimerTriggerParameters.RequiredValue);
    }
}

public readonly struct LevelTimerTriggerParameters(LevelTimerObservationType observationType, ComparisonType comparisonType, int requiredValue)
{
    public readonly LevelTimerObservationType ObservationType = observationType;
    public readonly ComparisonType ComparisonType = comparisonType;
    public readonly int RequiredValue = requiredValue;
}
