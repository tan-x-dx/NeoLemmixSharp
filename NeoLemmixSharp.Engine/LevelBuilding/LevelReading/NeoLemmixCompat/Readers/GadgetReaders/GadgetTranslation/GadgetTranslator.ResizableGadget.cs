using System.Runtime.InteropServices;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public sealed partial class GadgetTranslator
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
            GadgetBuilderId = archetypeData.GadgetArchetypeId,

            X = prototype.X,
            Y = prototype.Y,
            Orientation = orientation,
            FacingDirection = facingDirection
        };

        gadgetData.AddProperty(GadgetProperty.Behaviour, behaviour);
        gadgetData.AddProperty(GadgetProperty.Width, prototypeWidth);
        gadgetData.AddProperty(GadgetProperty.Height, prototypeHeight);

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders, archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            gadgetBuilder = new ResizeableGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId
            };
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }
}