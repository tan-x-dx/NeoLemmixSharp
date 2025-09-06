using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class LevelTimerObserverGadget : GadgetBase
{
    private readonly LevelTimerTrigger _levelTimerTrigger;

    public LevelTimerObserverGadget(LevelTimerTrigger levelTimerTrigger)
        : base(GadgetType.LevelTimerObserver)
    {
        _levelTimerTrigger = levelTimerTrigger;
    }

    public override GadgetState CurrentState { get; }

    public override void Tick()
    {
        _levelTimerTrigger.DetectTrigger();
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
        if (_levelTimerTriggerParameters.LevelTimerMatchesParameters())
        {
            LevelScreen.CauseAndEffectManager.RegisterCauseAndEffectData(new CauseAndEffectData(_outputSignalBehaviour.Id));
        }
    }
}

public readonly struct LevelTimerTriggerParameters(LevelTimerObservationType observationType, ComparisonType comparisonType, int requiredValue)
{
    private readonly LevelTimerObservationType _observationType = observationType;
    private readonly ComparisonType _comparisonType = comparisonType;
    private readonly int _requiredValue = requiredValue;

    public bool LevelTimerMatchesParameters()
    {
        var comparisonValue = _observationType == LevelTimerObservationType.SecondsElapsed
            ? LevelScreen.LevelTimer.EffectiveElapsedSeconds
            : LevelScreen.LevelTimer.EffectiveSecondsRemaining;

        return ComparisonMatches(comparisonValue);
    }

    private bool ComparisonMatches(int value)
    {
        return _comparisonType switch
        {
            ComparisonType.EqualTo => value == _requiredValue,
            ComparisonType.NotEqualTo => value != _requiredValue,
            ComparisonType.LessThan => value < _requiredValue,
            ComparisonType.LessThanOrEqual => value <= _requiredValue,
            ComparisonType.GreaterThan => value > _requiredValue,
            ComparisonType.GreaterThanOrEqual => value >= _requiredValue,

            _ => Helpers.ThrowUnknownEnumValueException<ComparisonType, bool>(_comparisonType),
        };
    }
}
