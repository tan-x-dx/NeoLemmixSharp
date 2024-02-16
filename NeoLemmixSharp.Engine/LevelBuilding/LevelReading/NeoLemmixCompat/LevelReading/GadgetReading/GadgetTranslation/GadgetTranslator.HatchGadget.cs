using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

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

        gadgetData.AddProperty(GadgetProperty.RawLemmingState, prototype.State);

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders, archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            gadgetBuilder = new HatchGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,

                SpawnX = archetypeData.TriggerX,
                SpawnY = archetypeData.TriggerY,

                HatchWidth = 2,
                HatchHeight = 2,
            };
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }
}