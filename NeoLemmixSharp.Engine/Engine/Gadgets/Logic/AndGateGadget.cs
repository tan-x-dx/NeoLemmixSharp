using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets.States;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Logic;

public sealed class AndGateGadget : IGadget
{
    private readonly AndGateState _currentState = new();

    public int Id { get; }

    public GadgetType Type => GadgetType.Logic;
    public Orientation Orientation => DownOrientation.Instance;
    public LevelPosition LevelPosition { get; }

    IGadgetState IGadget.CurrentState => _currentState;

    public AndGateGadget(int id, LevelPosition levelPosition)
    {
        Id = id;
        LevelPosition = levelPosition;
    }

    public void Tick()
    {
        _currentState.Tick();
    }

    private sealed class AndGateState : IGadgetState
    {
        public int AnimationFrame => 0;
        public void OnTransitionTo()
        {
        }

        public void Tick()
        {
        }

        public void OnTransitionFrom()
        {
        }
    }
}