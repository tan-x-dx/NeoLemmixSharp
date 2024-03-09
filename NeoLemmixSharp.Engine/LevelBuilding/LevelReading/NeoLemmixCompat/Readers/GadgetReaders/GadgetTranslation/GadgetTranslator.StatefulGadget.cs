using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public sealed partial class GadgetTranslator
{
    private void ProcessStatefulGadgetBuilder(
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
            ShouldRender = true,
            Orientation = orientation,
            FacingDirection = facingDirection
        };

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders, archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            gadgetBuilder = CreateStatefulGadgetBuilder();
        }

        _levelData.AllGadgetData.Add(gadgetData);

        return;

        StatefulGadgetBuilder CreateStatefulGadgetBuilder()
        {
            var spriteData = GetStitchedSpriteData(archetypeData);

            var gadgetStateData = new GadgetStateData[archetypeData.AnimationData.Count];
            var gadgetBehaviour = archetypeData.Behaviour.ToGadgetBehaviour()!;

            return new StatefulGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,
                GadgetBehaviour = gadgetBehaviour,
                AllGadgetStateData = gadgetStateData,

                SpriteData = spriteData
            };
        }
    }
}