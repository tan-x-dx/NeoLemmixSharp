using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchGadget : GadgetBase, IMoveableGadget
{
    private readonly HatchGadgetState[] _states;

    public HatchSpawnData HatchSpawnData { get; }
    public Point SpawnPointOffset { get; }

    private HatchGadgetState _currentState;

    public override HatchGadgetState CurrentState => _currentState;

    public HatchGadget(
        HatchGadgetState[] states,
        int initialStateIndex,
        HatchSpawnData hatchSpawnData,
        Point spawnPointOffset)
        : base(GadgetType.HatchGadget)
    {
        _states = states;
        _currentState = states[initialStateIndex];
        SpawnPointOffset = spawnPointOffset;
        HatchSpawnData = hatchSpawnData;
    }

    public override void Tick() { }

    public sealed override void SetState(int stateIndex)
    {
        _currentState = _states[stateIndex];
    }

    public bool CanReleaseLemmings()
    {
        return _currentState.Type == HatchGadgetStateType.Open &&
               HatchSpawnData.LemmingsToRelease > 0;
    }

    public void Move(Point delta)
    {
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(CurrentGadgetBounds.Position + delta);
    }

    public void SetPosition(Point position)
    {
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(position);
    }
}
