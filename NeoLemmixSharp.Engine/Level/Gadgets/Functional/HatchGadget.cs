using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class HatchGadget : GadgetBase, IMoveableGadget, IReactiveGadget, IControlledAnimationGadget
{
    private readonly LevelPosition _spawnPositionTranslation;

    public override Orientation Orientation => HatchSpawnData.Orientation;

    public LevelPosition SpawnPosition => LevelScreen.NormalisePosition(GadgetBounds.TopLeft + _spawnPositionTranslation);
    public HatchSpawnData HatchSpawnData { get; }
    public GadgetStateAnimationController AnimationController { get; }

    public HatchGadget(
        int id,
        RectangularHitBoxRegion gadgetBounds,
        IControlledAnimationGadgetRenderer? renderer,
        LevelPosition spawnPositionTranslation,
        HatchSpawnData hatchSpawnData,
        GadgetStateAnimationController animationController)
        : base(id, gadgetBounds, renderer)
    {
        _spawnPositionTranslation = spawnPositionTranslation;
        HatchSpawnData = hatchSpawnData;
        AnimationController = animationController;

        var topLeft = GadgetBounds.TopLeft;
        var bottomRight = GadgetBounds.BottomRight;

        TopLeftPixel = topLeft;
        BottomRightPixel = bottomRight;

        PreviousTopLeftPixel = topLeft;
        PreviousBottomRightPixel = bottomRight;
    }

    public override void Tick()
    {
    }

    public IGadgetInput? GetInputWithName(string inputName)
    {
        return null;
    }

    public void Move(int dx, int dy)
    {
        var newPosition = TopLeftPixel + new LevelPosition(dx, dy);

        UpdatePosition(newPosition);
    }

    public void SetPosition(int x, int y)
    {
        var newPosition = new LevelPosition(x, y);

        UpdatePosition(newPosition);
    }

    private void UpdatePosition(LevelPosition position)
    {
        PreviousTopLeftPixel = LevelScreen.NormalisePosition(TopLeftPixel);
        PreviousBottomRightPixel = LevelScreen.NormalisePosition(BottomRightPixel);

        position = LevelScreen.NormalisePosition(position);

        GadgetBounds.X = position.X;
        GadgetBounds.Y = position.Y;

        TopLeftPixel = LevelScreen.NormalisePosition(GadgetBounds.TopLeft);
        BottomRightPixel = LevelScreen.NormalisePosition(GadgetBounds.BottomRight);

        if (Renderer is not null)
        {
            LevelScreenRenderer.Instance.LevelRenderer.UpdateSpritePosition(Renderer);
        }
    }

    public bool CanReleaseLemmings()
    {
        return true;
    }
}