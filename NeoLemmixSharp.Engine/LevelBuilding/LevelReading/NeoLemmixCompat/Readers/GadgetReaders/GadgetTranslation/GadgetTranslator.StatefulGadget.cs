using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
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
        var gadgetData = new GadgetData
        {
            Id = gadgetId,
            GadgetBuilderId = archetypeData.GadgetArchetypeId,

            X = prototype.X,
            Y = prototype.Y,
            Orientation = DownOrientation.Instance,
            FacingDirection = FacingDirection.RightInstance
        };

        // gadgetData.AddProperty(GadgetProperty.Behaviour, behaviour);
        // gadgetData.AddProperty(GadgetProperty.Width, prototypeWidth);
        // gadgetData.AddProperty(GadgetProperty.Height, prototypeHeight);

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders, archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            var sprite = GetStitchedTextures(archetypeData, out var spriteWidth, out var spriteHeight);

            gadgetBuilder = new StatefulGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,

                Sprite = sprite
            };
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }
}