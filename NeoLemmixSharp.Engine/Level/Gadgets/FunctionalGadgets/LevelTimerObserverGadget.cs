using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class LevelTimerObserverGadget : GadgetBase
{
    private readonly LevelTimerGadgetState _offState;
    private readonly LevelTimerGadgetState _onState;
    private readonly OutputSignalBehaviour _outputSignalBehaviour;
    private readonly LevelTimerObservationType _observationType;
    private readonly ComparisonType _comparisonType;
    private readonly int _requiredValue;

    private LevelTimerGadgetState _currentState;

    public LevelTimerObserverGadget(
        LevelTimerGadgetState offState,
        LevelTimerGadgetState onState,
        OutputSignalBehaviour outputSignalBehaviour,
        LevelTimerObservationType observationType,
        ComparisonType comparisonType,
        int requiredValue)
        : base(GadgetType.LevelTimerObserver)
    {
        _offState = offState;
        _onState = onState;
        _currentState = _offState;
        _outputSignalBehaviour = outputSignalBehaviour;

        _observationType = observationType;
        _comparisonType = comparisonType;
        _requiredValue = requiredValue;

        _offState.SetParentGadget(this);
        _onState.SetParentGadget(this);
    }

    public override LevelTimerGadgetState CurrentState => _currentState;

    public override void Tick()
    {
        if (LevelTimerMatchesParameters())
        {
            LevelScreen.GadgetManager.RegisterCauseAndEffectData(_outputSignalBehaviour);
            _currentState = _onState;
        }
        else
        {
            _currentState = _offState;
        }

        CurrentState.Tick();
    }

    public bool LevelTimerMatchesParameters()
    {
        var comparisonValue = _observationType == LevelTimerObservationType.SecondsElapsed
            ? LevelScreen.LevelTimer.EffectiveElapsedSeconds
            : LevelScreen.LevelTimer.EffectiveSecondsRemaining;

        return _comparisonType.ComparisonMatches(comparisonValue, _requiredValue);
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
