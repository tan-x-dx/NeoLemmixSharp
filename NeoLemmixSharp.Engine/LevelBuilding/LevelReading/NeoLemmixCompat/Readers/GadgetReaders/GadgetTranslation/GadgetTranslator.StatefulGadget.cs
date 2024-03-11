using NeoLemmixSharp.Engine.Level.Gadgets.Actions;
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
            InitialStateId = 0,
            GadgetRenderMode = GetGadgetRenderMode(prototype),

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

            var gadgetStateData = CreateGadgetStateData();
            var gadgetBehaviour = archetypeData.Behaviour.ToGadgetBehaviour()!;

            return new StatefulGadgetBuilder
            {
                GadgetBuilderId = archetypeData.GadgetArchetypeId,
                GadgetBehaviour = gadgetBehaviour,
                AllGadgetStateData = gadgetStateData,

                SpriteData = spriteData
            };
        }

        GadgetStateData[] CreateGadgetStateData()
        {
            var emptyActions = Array.Empty<IGadgetAction>();

            var numberOfExtraStates = archetypeData.Behaviour.GetNumberOfExtraStates();

            var result = new GadgetStateData[1 + numberOfExtraStates];

            var baseState = new GadgetStateData
            {
                OnLemmingEnterActions = emptyActions,
                OnLemmingPresentActions = emptyActions,
                OnLemmingExitActions = emptyActions,

                NumberOfFrames = archetypeData.PrimaryAnimationFrameCount,
                TriggerData = new RectangularTriggerData
                {
                    TriggerX = archetypeData.TriggerX,
                    TriggerY = archetypeData.TriggerY,
                    TriggerWidth = archetypeData.TriggerWidth,
                    TriggerHeight = archetypeData.TriggerHeight
                }
            };

            var index = 0;
            result[index++] = baseState;
            var animationDataSpan = CollectionsMarshal.AsSpan(archetypeData.AnimationData);
            foreach (var animationData in animationDataSpan)
            {
                result[index++] = new GadgetStateData
                {
                    OnLemmingEnterActions = emptyActions,
                    OnLemmingPresentActions = emptyActions,
                    OnLemmingExitActions = emptyActions,

                    NumberOfFrames = animationData.NumberOfFrames,
                    TriggerData = null
                };
            }

            return result;
        }
    }
}