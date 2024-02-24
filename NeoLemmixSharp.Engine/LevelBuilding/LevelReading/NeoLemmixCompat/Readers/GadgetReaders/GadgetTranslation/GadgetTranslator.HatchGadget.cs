﻿using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public sealed partial class GadgetTranslator
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

        gadgetData.AddProperty(GadgetProperty.HatchGroupId, 0); // All NeoLemmix levels have precisely one hatch group
        gadgetData.AddProperty(GadgetProperty.Team, Team.AllItems[0]);
        gadgetData.AddProperty(GadgetProperty.RawLemmingState, prototype.State);
        gadgetData.AddProperty(GadgetProperty.LemmingCount, prototype.LemmingCount!.Value);

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders, archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            var sprite = GetStitchedTextures(archetypeData, out var spriteWidth, out var spriteHeight);

            gadgetBuilder = new HatchGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,

                SpawnX = archetypeData.TriggerX,
                SpawnY = archetypeData.TriggerY,

                HatchWidth = spriteWidth,
                HatchHeight = spriteHeight,

                Sprite = sprite
            };
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }
}