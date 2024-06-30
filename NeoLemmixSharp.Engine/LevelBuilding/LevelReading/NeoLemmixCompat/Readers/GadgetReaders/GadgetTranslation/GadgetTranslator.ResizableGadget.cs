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

        var behaviour = archetypeData.Behaviour.ToGadgetBehaviour();

        if (behaviour is null)
        {
            // TODO Things like one way walls

            return;
        }

        GetOrientationData(prototype, out var orientation, out var facingDirection);

        var gadgetData = new GadgetData
        {
            Id = gadgetId,
            Style = archetypeData.Style,
            GadgetPiece = archetypeData.GadgetPiece,
            GadgetBuilderId = archetypeData.GadgetArchetypeId,

            X = prototype.X,
            Y = prototype.Y,
            ShouldRender = true,
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

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders, archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            var spriteData = GetStitchedSpriteData(archetypeData);

            gadgetBuilder = new ResizeableGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,
                GadgetBehaviour = behaviour,

                SpriteData = spriteData
            };
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }
}