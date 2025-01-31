using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

public sealed class GadgetStateArchetypeData
{
    public required LevelPosition HitBoxOffset { get; init; }
    public required HitBoxData[] HitBoxData { get; init; }
    public HitBoxRegionDataArray RegionData;

    public required GadgetAnimationArchetypeData PrimaryAnimation { get; init; }
    public required int PrimaryAnimationStateTransitionIndex { get; init; }
    public required GadgetAnimationArchetypeData[] SecondaryAnimations { get; init; }

    public GadgetStateAnimationController GetAnimationController()
    {
        var primaryAnimationBehaviour = PrimaryAnimation.GetAnimationBehaviour();
        var secondaryAnimationBehaviours = GetSecondaryAnimationBehaviours();

        return new GadgetStateAnimationController(
            primaryAnimationBehaviour,
            PrimaryAnimationStateTransitionIndex,
            secondaryAnimationBehaviours);
    }

    private GadgetStateAnimationBehaviour[] GetSecondaryAnimationBehaviours()
    {
        var result = CollectionsHelper.GetArrayForSize<GadgetStateAnimationBehaviour>(SecondaryAnimations.Length);

        for (var i = 0; i < SecondaryAnimations.Length; i++)
        {
            result[i] = SecondaryAnimations[i].GetAnimationBehaviour();
        }

        return result;
    }
}

[InlineArray(4)]
public struct HitBoxRegionDataArray
{
    public HitBoxRegionData _x;
}