using NeoLemmixSharp.Common.Util.Collections;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public enum GadgetProperty
{
    Behaviour,
    HatchGroupId,
    TriggerX,
    TriggerY,
    TriggerWidth,
    TriggerHeight,
    Width,
    Height,
    Team,
    RawLemmingState,
    LemmingCount
}

public static class GadgetPropertyHelpers
{
    private const int NumberOfGadgetProperties = 11;

    private sealed class GadgetPropertyHasher : IPerfectHasher<GadgetProperty>
    {
        public int NumberOfItems => NumberOfGadgetProperties;

        public int Hash(GadgetProperty item) => (int)item;

        public GadgetProperty UnHash(int index) => (GadgetProperty)index;
    }

    public static SimpleDictionary<GadgetProperty, T> CreateSimpleDictionary<T>() => new(new GadgetPropertyHasher());
}