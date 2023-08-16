using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Gadgets.States;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.MetalGrates;

public sealed partial class MetalGrateGadget : IGadget
{
    private readonly DeactivatedState _deactivatedState;
    private readonly TransitionToActiveState _transitionToActiveState;
    private readonly ActivatedState _activatedState;
    private readonly TransitionToDeactivatedState _transitionToDeactivatedState;

    public int Id { get; }
    public GadgetType Type { get; }
    public Orientation Orientation { get; }
    public LevelPosition LevelPosition { get; }
    public IGadgetState CurrentState { get; private set; }

    public RectangularLevelRegion SpriteClip { get; }
    public IViewportObjectRenderer Renderer { get; }
    public RectangularLevelRegion HitBox => SpriteClip;

    public MetalGrateGadget(
        int id,
        Orientation orientation,
        LevelPosition levelPosition,
        RectangularLevelRegion spriteClip)
    {
        Id = id;
        Orientation = orientation;
        LevelPosition = levelPosition;
        SpriteClip = spriteClip;
        var hitBox = new MetalGrateHitBox(SpriteClip);

        _deactivatedState = new DeactivatedState(hitBox);
        _transitionToActiveState = new TransitionToActiveState(hitBox);
        _activatedState = new ActivatedState(hitBox);
        _transitionToDeactivatedState = new TransitionToDeactivatedState(hitBox);

        CurrentState = _activatedState;
    }

    public void Tick()
    {
    }
}