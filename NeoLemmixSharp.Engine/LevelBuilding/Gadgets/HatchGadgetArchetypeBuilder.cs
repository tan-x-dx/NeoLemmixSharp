using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class HatchGadgetArchetypeBuilder
{
    public static HatchGadget BuildGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
        LemmingManager lemmingManager,
        TribeManager tribeManager)
    {
        var hatchGadgetId = gadgetData.GetProperty(GadgetProperty.HatchGroupId);
        var tribeId = gadgetData.GetProperty(GadgetProperty.TribeId);
        var rawLemmingState = (uint)gadgetData.GetProperty(GadgetProperty.RawLemmingState);
        var lemmingCount = gadgetData.GetProperty(GadgetProperty.Count);

        var spawnPointOffset = GetSpawnPointOffset(gadgetArchetypeData, gadgetData);

        var hatchSpawnData = new HatchSpawnData(
            hatchGadgetId,
            tribeManager.AllItems[tribeId],
            rawLemmingState,
            gadgetData.Orientation,
            gadgetData.FacingDirection,
            lemmingCount);

        var gadgetName = GadgetBuildingHelpers.GetGadgetName(gadgetArchetypeData, gadgetData);
        var gadgetBounds = GadgetBuildingHelpers.CreateGadgetBounds(gadgetArchetypeData, gadgetData);
        var gadgetStates = BuildGadgetStates(gadgetArchetypeData, gadgetData, gadgetBounds, tribeManager);

        return new HatchGadget(
            gadgetName,
            gadgetStates,
            gadgetData.InitialStateId,
            hatchSpawnData,
            spawnPointOffset)
        {
            Id = gadgetData.Id,
            Orientation = gadgetData.Orientation,
            FacingDirection = FacingDirection.Right, // Hatches do not flip according to facing direction

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    private static Point GetSpawnPointOffset(GadgetArchetypeData gadgetArchetypeData, GadgetData gadgetData)
    {
        var rawSpawnPoint = GetRawSpawnPoint();

        var dihedralTransformation = new DihedralTransformation(
            gadgetData.Orientation,
            gadgetData.FacingDirection);
        return dihedralTransformation.Transform(rawSpawnPoint, gadgetArchetypeData.BaseSpriteSize);

        Point GetRawSpawnPoint()
        {
            var spawnPointMiscData = gadgetArchetypeData.GetMiscData(GadgetArchetypeMiscDataType.SpawnPointOffset);

            return IO.ReadWriteHelpers.DecodePoint(spawnPointMiscData);
        }
    }

    private static GadgetState[] BuildGadgetStates(GadgetArchetypeData gadgetArchetypeData, GadgetData gadgetData, GadgetBounds gadgetBounds, TribeManager tribeManager)
    {
        throw new NotImplementedException();
    }
}