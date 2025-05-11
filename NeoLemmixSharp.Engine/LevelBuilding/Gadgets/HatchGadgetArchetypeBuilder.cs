using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public sealed class HatchGadgetArchetypeBuilder : IGadgetArchetypeBuilder
{
    public required StyleIdentifier StyleName { get; init; }
    public required PieceIdentifier PieceName { get; init; }

    public required Point SpawnPosition { get; init; }

    public required SpriteArchetypeData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetRendererBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TeamManager teamManager)
    {
        var hatchGadgetId = gadgetData.GetProperty(GadgetProperty.HatchGroupId);
        var teamId = gadgetData.GetProperty(GadgetProperty.TeamId);
        var rawLemmingState = (uint)gadgetData.GetProperty(GadgetProperty.RawLemmingState);
        var lemmingCount = gadgetData.GetProperty(GadgetProperty.Count);

        var dihedralTransformation = new DihedralTransformation(
            gadgetData.Orientation,
            FacingDirection.Right); // Hatches do not flip according to facing direction

        var transformedSize = dihedralTransformation.Transform(SpriteData.BaseSpriteSize);
        var spawnPointOffset = dihedralTransformation.Transform(SpawnPosition, SpriteData.BaseSpriteSize);

        var currentGadgetBounds = new GadgetBounds
        {
            Position = gadgetData.Position,
            Width = transformedSize.W,
            Height = transformedSize.H
        };

        var gadgetRenderer = gadgetSpriteBuilder.BuildStatefulGadgetRenderer(this, gadgetData);

        var hatchSpawnData = new HatchSpawnData(
            hatchGadgetId,
            teamManager.AllItems[teamId],
            rawLemmingState,
            gadgetData.Orientation,
            gadgetData.FacingDirection,
            lemmingCount);

        /*   var gadgetAnimationController = new GadgetStateAnimationController(
               new GadgetStateAnimationBehaviour(SpriteData.SpriteWidth, SpriteData.SpriteHeight, 0, 0, 0, SpriteData.FrameCountsPerLayer[0], GadgetSecondaryAnimationAction.Play),
               -1,
               Array.Empty<GadgetStateAnimationBehaviour>());*/

        var result = new HatchGadget(
            hatchSpawnData,
            spawnPointOffset,
            null!)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,

            CurrentGadgetBounds = currentGadgetBounds,

            IsFastForward = false
        };

        //  gadgetRenderer?.SetGadget(result);

        return result;
    }
}