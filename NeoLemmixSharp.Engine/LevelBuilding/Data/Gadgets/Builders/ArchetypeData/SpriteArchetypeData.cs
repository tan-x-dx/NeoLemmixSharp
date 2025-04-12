using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class SpriteArchetypeData
{
    public required Size BaseSpriteSize { get; init; }
    public required int NumberOfLayers { get; init; }
    public required int MaxNumberOfFrames { get; init; }

    public required StateSpriteArchetypeData[] SpriteArchetypeDataForStates { get; init; }

    public AnimationController CreateAnimationController(
        int stateIndex,
        GadgetBounds currentGadgetBounds,
        GadgetBounds previousGadgetBounds)
    {
        var animationBehaviours = SpriteArchetypeDataForStates[stateIndex].CreateAnimationBehaviours();

        return new AnimationController(
            animationBehaviours,
            currentGadgetBounds,
            previousGadgetBounds);
    }
}

public readonly struct StateSpriteArchetypeData
{
    public required AnimationBehaviourArchetypeData[] AnimationData { get; init; }

    public AnimationBehaviour[] CreateAnimationBehaviours()
    {
        var result = new AnimationBehaviour[AnimationData.Length];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = AnimationData[i].CreateAnimationBehaviour();
        }

        return result;
    }
}

public readonly struct AnimationBehaviourArchetypeData
{
    public required AnimationParameters AnimationParameters { get; init; }
    public required NineSliceDataThing[] NineSliceData { get; init; }
    public required int InitialFrame { get; init; }
    public required int NextGadgetState { get; init; }

    public AnimationBehaviour CreateAnimationBehaviour()
    {
        return new AnimationBehaviour(AnimationParameters, NineSliceData, InitialFrame, NextGadgetState);
    }
}