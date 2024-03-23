using NeoLemmixSharp.Common.Util.Collections;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public enum GadgetProperty
{
    HatchGroupId,
    TeamId,
    Width,
    Height,
    RawLemmingState,
    LemmingCount,
    InitialAnimationFrame
}

public static class GadgetPropertyHelpers
{
    private const int NumberOfGadgetProperties = 6;

    private sealed class GadgetPropertyHasher : IPerfectHasher<GadgetProperty>
    {
        public int NumberOfItems => NumberOfGadgetProperties;

        public int Hash(GadgetProperty item) => (int)item;

        public GadgetProperty UnHash(int index) => (GadgetProperty)index;
    }

    public static SimpleDictionary<GadgetProperty, int> CreateSimpleIntDictionary() => new(new GadgetPropertyHasher());
}