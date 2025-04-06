using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class HatchGadgetArchetypeBuilder : IGadgetArchetypeBuilder
{
    public required string StyleName { get; init; }
    public required string PieceName { get; init; }

    public required Point SpawnPosition { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
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

        var transformedSize = dihedralTransformation.Transform(SpriteData.SpriteSize);
        var spawnPointOffset = dihedralTransformation.Transform(SpawnPosition, SpriteData.SpriteSize);

        var currentGadgetBounds = new GadgetBounds
        {
            Position = gadgetData.Position,
            Width = transformedSize.W,
            Height = transformedSize.H
        };
        var previousGadgetBounds = new GadgetBounds(currentGadgetBounds);

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
            PreviousGadgetBounds = previousGadgetBounds,

            IsFastForward = false
        };

        //  gadgetRenderer?.SetGadget(result);

        return result;
    }
}