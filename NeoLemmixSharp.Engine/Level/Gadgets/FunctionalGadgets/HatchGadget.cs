using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class HatchGadget : GadgetBase, IMoveableGadget
{
    public HatchSpawnData HatchSpawnData { get; }
    public Point SpawnPointOffset { get; }

    public HatchGadget(
        HatchSpawnData hatchSpawnData,
        Point spawnPointOffset,
        AnimationController animationController)
        : base(0)
    {
        SpawnPointOffset = spawnPointOffset;
        HatchSpawnData = hatchSpawnData;

        CurrentAnimationController = animationController;
    }

    public override void Tick() => CurrentAnimationController.Tick();

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
}
