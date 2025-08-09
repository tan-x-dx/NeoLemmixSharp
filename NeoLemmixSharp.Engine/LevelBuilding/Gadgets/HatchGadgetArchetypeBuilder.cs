using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget.Hatch;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;
/*
public sealed class HatchGadgetArchetypeBuilder
{
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public HatchGadgetArchetypeBuilder(
        List<GadgetTrigger> gadgetTriggers,
        List<GadgetBehaviour> gadgetBehaviours)
    {
        _gadgetTriggers = gadgetTriggers;
        _gadgetBehaviours = gadgetBehaviours;
    }

    public HatchGadget BuildGadget(
        HatchGadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData,
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
            Id = gadgetData.Identifier.GadgetId,
            Orientation = gadgetData.Orientation,
            FacingDirection = FacingDirection.Right, // Hatches do not flip according to facing direction

            CurrentGadgetBounds = gadgetBounds,

            IsFastForward = true
        };
    }

    private Point GetSpawnPointOffset(HatchGadgetArchetypeData gadgetArchetypeData, GadgetData gadgetData)
    {
        var rawSpawnPoint = gadgetArchetypeData.SpawnOffset;

        var dihedralTransformation = new DihedralTransformation(
            gadgetData.Orientation,
            gadgetData.FacingDirection);
        return dihedralTransformation.Transform(rawSpawnPoint, gadgetArchetypeData.BaseSpriteSize);
    }

    private GadgetState[] BuildGadgetStates(HatchGadgetArchetypeData gadgetArchetypeData, GadgetData gadgetData, GadgetBounds gadgetBounds, TribeManager tribeManager)
    {
        throw new NotImplementedException();
    }
}
*/