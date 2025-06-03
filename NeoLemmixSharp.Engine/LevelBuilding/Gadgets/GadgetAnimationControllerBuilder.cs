using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class GadgetAnimationControllerBuilder
{
    public static AnimationController BuildAnimationController(
        AnimationLayerArchetypeData[] animationLayerData,
        GadgetBounds gadgetBounds)
    {
        var animationLayers = BuildAnimationLayers(animationLayerData);

        return new AnimationController(animationLayers, gadgetBounds);
    }

    private static AnimationLayer[] BuildAnimationLayers(AnimationLayerArchetypeData[] animationLayerData)
    {
        var result = CollectionsHelper.GetArrayForSize<AnimationLayer>(animationLayerData.Length);

        for (var i = 0; i < result.Length; i++)
        {
            var animationLayerArchetypeData = animationLayerData[i];

            var nineSliceData = BuildNineSliceData(animationLayerArchetypeData);

            result[i] = new AnimationLayer(
                animationLayerArchetypeData.AnimationLayerParameters,
                nineSliceData,
                animationLayerArchetypeData.InitialFrame,
                animationLayerArchetypeData.NextGadgetState);
        }

        return result;
    }

    private static NineSliceRenderer[] BuildNineSliceData(AnimationLayerArchetypeData animationLayerArchetypeData)
    {
        throw new NotImplementedException();
    }
}
