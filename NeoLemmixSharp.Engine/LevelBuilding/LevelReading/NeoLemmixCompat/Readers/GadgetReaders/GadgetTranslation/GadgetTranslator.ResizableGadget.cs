using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public readonly ref partial struct GadgetTranslator
{
    private void ProcessResizeableGadgetBuilder(
        NeoLemmixGadgetArchetypeData archetypeData,
        NeoLemmixGadgetData prototype,
        int gadgetId)
    {
        if (!prototype.Width.HasValue ||
            !prototype.Height.HasValue)
            throw new InvalidOperationException("Dimensions not specified for resizeable gadget!");

        var prototypeWidth = prototype.Width.Value;
        var prototypeHeight = prototype.Height.Value;

        GetOrientationData(prototype, out var orientation, out var facingDirection);

        var gadgetData = new GadgetData
        {
            Id = gadgetId,
            Style = archetypeData.Style,
            GadgetPiece = archetypeData.GadgetPiece,
            GadgetBuilderId = archetypeData.GadgetArchetypeId,

            X = prototype.X,
            Y = prototype.Y,
            InitialStateId = 0,
            GadgetRenderMode = GetGadgetRenderMode(prototype),

            Orientation = orientation,
            FacingDirection = facingDirection
        };

        gadgetData.SetProperty(GadgetProperty.Width, prototypeWidth);
        gadgetData.SetProperty(GadgetProperty.Height, prototypeHeight);
        if (archetypeData.AnimationData.Count > 0)
        {
            gadgetData.SetProperty(GadgetProperty.InitialAnimationFrame, archetypeData.AnimationData[0].InitialFrame);
        }

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders,
            archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            var spriteData = GetStitchedSpriteData(archetypeData);

            gadgetBuilder = new ResizeableGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,
                ArchetypeData = new GadgetStateArchetypeData
                {
                    OnLemmingEnterActions = [],
                    OnLemmingPresentActions = [],
                    OnLemmingExitActions = [],
                    TriggerType = TriggerType.Rectangular,
                    TriggerData =
                        GetTriggerData(archetypeData, spriteData.SpriteWidth, spriteData.SpriteHeight),
                    PrimaryAnimation = null,
                    PrimaryAnimationStateTransitionIndex = 0,
                    SecondaryAnimations = null,
                },

                SpriteData = spriteData
            };
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }

    private static LevelPosition[] GetTriggerData(
        NeoLemmixGadgetArchetypeData archetypeData,
        int spriteWidth,
        int spriteHeight)
    {
        var result = new[]
        {
            new LevelPosition(
                archetypeData.TriggerX,
                archetypeData.TriggerY),
            new LevelPosition(
                spriteWidth - (archetypeData.TriggerX + archetypeData.TriggerWidth),
                spriteHeight - (archetypeData.TriggerY + archetypeData.TriggerHeight))
        };

        return result;
    }
}