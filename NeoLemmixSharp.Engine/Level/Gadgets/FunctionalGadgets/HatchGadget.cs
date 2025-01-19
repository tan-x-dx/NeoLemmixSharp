using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public sealed class HatchGadget : GadgetBase, 
    IAnimationControlledGadget,
    IMoveableGadget
{
    private GadgetLayerRenderer _renderer;

    public  LevelPosition SpawnPointOffset { get; }
    public HatchSpawnData HatchSpawnData { get; }
    public GadgetStateAnimationController AnimationController { get; }

    public override GadgetLayerRenderer Renderer => _renderer;

    public HatchGadget(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        HatchSpawnData hatchSpawnData,
        LevelPosition spawnPointOffset,
        GadgetStateAnimationController animationController)
        : base(id, orientation, gadgetBounds)
    {
        SpawnPointOffset = spawnPointOffset;
        HatchSpawnData = hatchSpawnData;
        AnimationController = animationController;
    }

    public override void Tick() { }

    public bool CanReleaseLemmings()
    {
        return true;
    }

    public void Move(int dx, int dy)
    {
        _previousGadgetBounds.SetFrom(_currentGadgetBounds);

        _currentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(_currentGadgetBounds.X + dx);
        _currentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(_currentGadgetBounds.Y + dy);
    }

    public void SetPosition(int x, int y)
    {
        _previousGadgetBounds.SetFrom(_currentGadgetBounds);

        _currentGadgetBounds.X = LevelScreen.HorizontalBoundaryBehaviour.Normalise(x);
        _currentGadgetBounds.Y = LevelScreen.VerticalBoundaryBehaviour.Normalise(y);
    }
}
