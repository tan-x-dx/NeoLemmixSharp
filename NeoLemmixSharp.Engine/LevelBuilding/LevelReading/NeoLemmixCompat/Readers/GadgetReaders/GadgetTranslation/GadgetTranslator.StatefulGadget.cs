using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public readonly ref partial struct GadgetTranslator
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

        if (archetypeData.AnimationData.Count > 0)
        {
            gadgetData.SetProperty(GadgetProperty.InitialAnimationFrame, archetypeData.AnimationData[0].InitialFrame);
        }

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(
            _levelData.AllGadgetBuilders,
            archetypeData.GadgetArchetypeId,
            out var exists);

        if (!exists)
        {
            gadgetBuilder = CreateStatefulGadgetBuilder(archetypeData);
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }

    private StatefulGadgetBuilder CreateStatefulGadgetBuilder(NeoLemmixGadgetArchetypeData archetypeData)
    {
        var spriteData = GetStitchedSpriteData(archetypeData);

        var gadgetStateData = archetypeData.GetGadgetStates(spriteData);

        return new StatefulGadgetBuilder
        {
            GadgetBuilderId = archetypeData.GadgetArchetypeId,
            AllGadgetStateData = gadgetStateData,

            SpriteData = spriteData
        };
    }
}