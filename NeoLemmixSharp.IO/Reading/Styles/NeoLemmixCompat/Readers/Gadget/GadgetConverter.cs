using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HatchGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

internal static class GadgetConverter
{
    public static GadgetArchetypeData ConvertToGadgetArchetypeData(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {
        var specificationData = CreateGadgetSpecificationData(neoLemmixGadgetArchetypeData);

        var result = new GadgetArchetypeData
        {
            StyleIdentifier = neoLemmixGadgetArchetypeData.StyleIdentifier,
            PieceIdentifier = neoLemmixGadgetArchetypeData.GadgetPieceIdentifier,
            GadgetName = neoLemmixGadgetArchetypeData.GadgetPieceIdentifier.ToString(),
            BaseSpriteSize = default,
            SpecificationData = specificationData,
        };

        return result;
    }

    private static IGadgetArchetypeSpecificationData CreateGadgetSpecificationData(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {
        var gadgetBehaviour = neoLemmixGadgetArchetypeData.Behaviour;

        if (gadgetBehaviour is NeoLemmixGadgetBehaviour.Entrance)
            return CreateHatchGadgetArchetypeSpecificationData(neoLemmixGadgetArchetypeData);

        return CreateHitBoxGadgetArchetypeSpecificationData(neoLemmixGadgetArchetypeData);
    }

    private static HatchGadgetArchetypeSpecificationData CreateHatchGadgetArchetypeSpecificationData(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {
        var spawnOffset = new Point(neoLemmixGadgetArchetypeData.TriggerX, neoLemmixGadgetArchetypeData.TriggerY);

        var hatchGadgetStates = CreateHatchGadgetStates(neoLemmixGadgetArchetypeData);

        return new HatchGadgetArchetypeSpecificationData
        {
            SpawnOffset = spawnOffset,

            GadgetStates = hatchGadgetStates
        };
    }

    private static HatchGadgetStateArchetypeData[] CreateHatchGadgetStates(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {
        var result = new HatchGadgetStateArchetypeData[EngineConstants.ExpectedNumberOfHatchGadgetStates];
        result[0] = new HatchGadgetStateArchetypeData
        {
            StateName = "Closed",

            InnateTriggers = [],
            InnateBehaviours = [],
            TriggerBehaviourLinks = []
        };
        result[1] = new HatchGadgetStateArchetypeData
        {
            StateName = "Opening",

            InnateTriggers = [],
            InnateBehaviours = [],
            TriggerBehaviourLinks = []
        };
        result[2] = new HatchGadgetStateArchetypeData
        {
            StateName = "Open",

            InnateTriggers = [],
            InnateBehaviours = [],
            TriggerBehaviourLinks = []
        };
        result[3] = new HatchGadgetStateArchetypeData
        {
            StateName = "Closing",

            InnateTriggers = [],
            InnateBehaviours = [],
            TriggerBehaviourLinks = []
        };

        return result;
    }

    private static HitBoxGadgetArchetypeSpecificationData CreateHitBoxGadgetArchetypeSpecificationData(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {
        var hitBoxGadgetStates = CreateHitBoxGadgetStates(neoLemmixGadgetArchetypeData);

        return new HitBoxGadgetArchetypeSpecificationData
        {
            ResizeType = neoLemmixGadgetArchetypeData.ResizeType,
            NineSliceData = new RectangularRegion(),

            GadgetStates = hitBoxGadgetStates
        };
    }

    private static HitBoxGadgetStateArchetypeData[] CreateHitBoxGadgetStates(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {

        return [];
    }
}
