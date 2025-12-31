using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
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

        var baseSpriteSize = DetermineBaseSpriteSize(neoLemmixGadgetArchetypeData);

        var result = new GadgetArchetypeData
        {
            StyleIdentifier = neoLemmixGadgetArchetypeData.StyleIdentifier,
            PieceIdentifier = neoLemmixGadgetArchetypeData.GadgetPieceIdentifier,
            GadgetName = neoLemmixGadgetArchetypeData.GadgetPieceIdentifier.ToString(),
            TextureFilePath = neoLemmixGadgetArchetypeData.AnimationData.First(x => !string.IsNullOrWhiteSpace(x.TextureFilePath)).TextureFilePath!,
            BaseSpriteSize = baseSpriteSize,
            SpecificationData = specificationData,
        };

        return result;
    }

    private static Size DetermineBaseSpriteSize(NeoLemmixGadgetArchetypeData neoLemmixGadgetArchetypeData)
    {
        if (neoLemmixGadgetArchetypeData.HasSpriteSizeSpecified)
            return new Size(neoLemmixGadgetArchetypeData.SpriteWidth, neoLemmixGadgetArchetypeData.SpriteHeight);

        foreach (var animationData in neoLemmixGadgetArchetypeData.AnimationData)
        {
            var gadgetTexture = animationData.TryGetOrLoadGadgetTexture(neoLemmixGadgetArchetypeData.StyleIdentifier, neoLemmixGadgetArchetypeData.GadgetPieceIdentifier);

            if (gadgetTexture is not null)
            {
                return SpriteHelpers.DetermineSpriteSize(animationData.TextureFilePath!, gadgetTexture, 1, animationData.FrameCount);
            }
        }

        return default;
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
            Type = HatchGadgetStateType.Closed,

            InnateTriggers = [],
            InnateBehaviours = [],
            TriggerBehaviourLinks = []
        };
        result[1] = new HatchGadgetStateArchetypeData
        {
            StateName = "Opening",
            Type = HatchGadgetStateType.Opening,

            InnateTriggers = [],
            InnateBehaviours = [],
            TriggerBehaviourLinks = []
        };
        result[2] = new HatchGadgetStateArchetypeData
        {
            StateName = "Open",
            Type = HatchGadgetStateType.Open,

            InnateTriggers = [],
            InnateBehaviours = [],
            TriggerBehaviourLinks = []
        };
        result[3] = new HatchGadgetStateArchetypeData
        {
            StateName = "Closing",
            Type = HatchGadgetStateType.Closing,

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
