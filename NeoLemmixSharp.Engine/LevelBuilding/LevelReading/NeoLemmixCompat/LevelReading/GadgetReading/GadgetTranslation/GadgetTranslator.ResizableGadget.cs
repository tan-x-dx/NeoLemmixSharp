using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading.GadgetTranslation;

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
            // TODO

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

        var gadgetBuilder = new ResizeableGadgetBuilder
        {
            GadgetBuilderId = archetypeData.GadgetArchetypeId
        };

        _gadgetBuilders.Add(gadgetBuilder);
        _gadgetDatas.Add(gadgetData);
    }
}