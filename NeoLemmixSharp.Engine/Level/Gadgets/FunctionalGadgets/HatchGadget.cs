using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class HatchGadget : GadgetBase, IMoveableGadget
{
    public HatchSpawnData HatchSpawnData { get; }
    public LevelPosition SpawnPointOffset { get; }

    public HatchGadget(
        HatchSpawnData hatchSpawnData,
        LevelPosition spawnPointOffset,
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
        PreviousGadgetBounds.SetFrom(CurrentGadgetBounds);

        CurrentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(CurrentGadgetBounds.X + dx);
        CurrentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(CurrentGadgetBounds.Y + dy);
    }

    public void SetPosition(int x, int y)
    {
        PreviousGadgetBounds.SetFrom(CurrentGadgetBounds);

        CurrentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(x);
        CurrentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(y);
    }
}
