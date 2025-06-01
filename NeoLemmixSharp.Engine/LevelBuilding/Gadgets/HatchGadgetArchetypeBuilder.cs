using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class HatchGadgetArchetypeBuilder
{
    public static GadgetBase BuildGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var hatchGadgetId = gadgetData.GetProperty(GadgetProperty.HatchGroupId);
        var tribeId = gadgetData.GetProperty(GadgetProperty.TribeId);
        var rawLemmingState = (uint)gadgetData.GetProperty(GadgetProperty.RawLemmingState);
        var lemmingCount = gadgetData.GetProperty(GadgetProperty.Count);

        var dihedralTransformation = new DihedralTransformation(
            gadgetData.Orientation,
            FacingDirection.Right); // Hatches do not flip according to facing direction

       //var transformedSize = dihedralTransformation.Transform(SpriteData.BaseSpriteSize);
        //var spawnPointOffset = dihedralTransformation.Transform(SpawnPosition, SpriteData.BaseSpriteSize);

        /*var currentGadgetBounds = new GadgetBounds
        {
            Position = gadgetData.Position,
            Width = transformedSize.W,
            Height = transformedSize.H
        };*/

       // var gadgetRenderer = gadgetSpriteBuilder.BuildStatefulGadgetRenderer(gadgetData);

        var hatchSpawnData = new HatchSpawnData(
            hatchGadgetId,
            tribeManager.AllItems[tribeId],
            rawLemmingState,
            gadgetData.Orientation,
            gadgetData.FacingDirection,
            lemmingCount);

        /*   var gadgetAnimationController = new GadgetStateAnimationController(
               new GadgetStateAnimationBehaviour(SpriteData.SpriteWidth, SpriteData.SpriteHeight, 0, 0, 0, SpriteData.FrameCountsPerLayer[0], GadgetSecondaryAnimationAction.Play),
               -1,
               Array.Empty<GadgetStateAnimationBehaviour>());*/

        return null;
        /*
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

        return result;*/
    }
}