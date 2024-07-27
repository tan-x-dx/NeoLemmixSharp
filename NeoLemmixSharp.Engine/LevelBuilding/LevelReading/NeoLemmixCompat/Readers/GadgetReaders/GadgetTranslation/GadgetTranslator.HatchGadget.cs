using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

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
            Style = archetypeData.Style,
            GadgetPiece = archetypeData.GadgetPiece,

            GadgetBuilderId = archetypeData.GadgetArchetypeId,

            X = prototype.X,
            Y = prototype.Y,
            InitialStateId = 0,
            GadgetRenderMode = GetGadgetRenderMode(prototype),

            Orientation = orientation,
            FacingDirection = facingDirection,
        };

        gadgetData.SetProperty(GadgetProperty.HatchGroupId, 0); // All NeoLemmix levels have precisely one hatch group
        gadgetData.SetProperty(GadgetProperty.TeamId, LevelConstants.ClassicTeamId); // All NeoLemmix levels use the default team
        gadgetData.SetProperty(GadgetProperty.RawLemmingState, (int)prototype.State);
        gadgetData.SetProperty(GadgetProperty.LemmingCount, prototype.LemmingCount!.Value);

        ref var gadgetBuilder = ref CollectionsMarshal.GetValueRefOrAddDefault(_levelData.AllGadgetBuilders, archetypeData.GadgetArchetypeId, out var exists);

        if (!exists)
        {
            var spriteData = GetStitchedSpriteData(archetypeData);

            gadgetBuilder = new HatchGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,

                SpawnX = archetypeData.TriggerX,
                SpawnY = archetypeData.TriggerY,

                SpriteData = spriteData
            };
        }

        _levelData.AllGadgetData.Add(gadgetData);
    }
}