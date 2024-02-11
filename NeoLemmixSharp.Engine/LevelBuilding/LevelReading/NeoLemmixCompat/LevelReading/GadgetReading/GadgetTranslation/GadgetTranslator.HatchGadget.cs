using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading.GadgetTranslation;

public readonly ref partial struct GadgetTranslator
{
    private void ProcessHatchGadgetBuilder(
        NeoLemmixGadgetArchetypeData archetypeData,
        NeoLemmixGadgetData prototype,
        int gadgetId)
    {
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

        var gadgetBuilder = new HatchGadgetBuilder
        {
            GadgetBuilderId = archetypeData.GadgetArchetypeId,
            HatchWidth = 2,
            HatchHeight = 2,
        };

        _gadgetBuilders.Add(gadgetBuilder);
        _gadgetDatas.Add(gadgetData);
    }
}