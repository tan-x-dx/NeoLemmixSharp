using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets.States;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public sealed class GenericGadget : IGadget
{
    private readonly IGadgetState[] _states;

    private int _currentStateIndex;

    public int Id { get; }
    public GadgetType Type => GadgetType.None;
    public Orientation Orientation { get; }
    public LevelPosition LevelPosition { get; }
    public IGadgetState CurrentState => _states[_currentStateIndex];

    public GenericGadget(
        int id,
        Orientation orientation,
        LevelPosition levelPosition,
        IGadgetState[] states)
    {
        Id = id;
        Orientation = orientation;
        LevelPosition = levelPosition;
        _states = states;
    }

    public void Tick()
    {
        CurrentState.Tick();
    }


}