using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using static NeoLemmixSharp.Engine.Level.Gadgets.Animations.TeamColors;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class SpriteArchetypeData
{
    public required Size BaseSpriteSize { get; init; }
    public required int NumberOfLayers { get; init; }
    public required int MaxNumberOfFrames { get; init; }

    public required StateSpriteArchetypeData[] SpriteArchetypeDataForStates { get; init; }

    public AnimationController CreateAnimationController(
        int stateIndex,
        GadgetBounds currentGadgetBounds)
    {
        var animationLayers = SpriteArchetypeDataForStates[stateIndex].CreateAnimationLayers();

        return new AnimationController(
            animationLayers,
            currentGadgetBounds);
    }
}

public readonly struct StateSpriteArchetypeData
{
    public required AnimationLayerArchetypeData[] AnimationData { get; init; }

    public AnimationLayer[] CreateAnimationLayers()
    {
        var result = new AnimationLayer[AnimationData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = AnimationData[i].CreateAnimationBehaviour();
        }

        return result;
    }
}

public readonly struct AnimationLayerArchetypeData
{
    public required AnimationLayerParameters AnimationLayerParameters { get; init; }
    public required int InitialFrame { get; init; }
    public required int NextGadgetState { get; init; }
    public required TeamColorChooser ColorChooser { get; init; }
    public required NineSliceDataThing[] NineSliceData { get; init; }

    public AnimationLayer CreateAnimationBehaviour()
    {
        return new AnimationLayer(AnimationLayerParameters, ColorChooser, NineSliceData, InitialFrame, NextGadgetState);
    }
}