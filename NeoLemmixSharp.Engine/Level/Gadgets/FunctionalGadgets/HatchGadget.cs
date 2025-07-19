using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class HatchGadget : GadgetBase, IMoveableGadget
{
    public HatchSpawnData HatchSpawnData { get; }
    public Point SpawnPointOffset { get; }

    public HatchGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        HatchSpawnData hatchSpawnData,
        Point spawnPointOffset)
        : base(gadgetName)
    {
        SpawnPointOffset = spawnPointOffset;
        HatchSpawnData = hatchSpawnData;
    }

    public override void Tick() { }

    public bool CanReleaseLemmings()
    {
        return true;
    }

    public void Move(int dx, int dy)
    {
        var delta = new Point(dx, dy);
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(CurrentGadgetBounds.Position + delta);
    }

    public void SetPosition(int x, int y)
    {
        var newPosition = new Point(x, y);
        CurrentGadgetBounds.Position = LevelScreen.NormalisePosition(newPosition);
    }

    public override GadgetState CurrentState => throw new NotImplementedException();

    public override void SetNextState(int stateIndex)
    {
        throw new NotImplementedException();
    }
}
