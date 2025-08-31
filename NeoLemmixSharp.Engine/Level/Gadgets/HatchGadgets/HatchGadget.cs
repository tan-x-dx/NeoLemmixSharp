using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchGadget : GadgetBase, IMoveableGadget
{
    private readonly HatchGadgetState[] _states;

    public HatchSpawnData HatchSpawnData { get; }
    public Point SpawnPointOffset { get; }

    public HatchGadget(
        HatchGadgetState[] states,
        int initialStateIndex,
        HatchSpawnData hatchSpawnData,
        Point spawnPointOffset)
    {
        _states = states;
        SpawnPointOffset = spawnPointOffset;
        HatchSpawnData = hatchSpawnData;
    }

    public override void Tick() { }

    public bool CanReleaseLemmings()
    {
        return true;
    }

    public void Move(Point delta)
    {
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(CurrentGadgetBounds.Position + delta);
    }

    public void SetPosition(Point position)
    {
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(position);
    }

    public override GadgetState CurrentState => throw new NotImplementedException();

    public override void SetNextState(int stateIndex)
    {
        throw new NotImplementedException();
    }
}
