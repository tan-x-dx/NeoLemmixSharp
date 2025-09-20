using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class LevelTimerObserverGadget : GadgetBase
{
    private readonly LevelTimerTrigger _trigger;

    private readonly LevelTimerGadgetState _offState;
    private readonly LevelTimerGadgetState _onState;

    private LevelTimerGadgetState _currentState;

    public LevelTimerObserverGadget(
        LevelTimerTrigger trigger,
        LevelTimerGadgetState offState,
        LevelTimerGadgetState onState)
        : base(GadgetType.LevelTimerObserver)
    {
        _trigger = trigger;
        _offState = offState;
        _onState = onState;
        _currentState = _offState;

        _offState.SetParentGadget(this);
        _onState.SetParentGadget(this);
    }

    public override LevelTimerGadgetState CurrentState => _currentState;

    public override void Tick()
    {
        CurrentState.Tick();
    }

    public sealed override void SetState(int stateIndex)
    {
        var state = stateIndex & 1;
        _currentState = state == 0
            ? _offState
            : _onState;
    }
}

public sealed class LevelTimerGadgetState : GadgetState
{
    public override GadgetRenderer Renderer { get; }
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
