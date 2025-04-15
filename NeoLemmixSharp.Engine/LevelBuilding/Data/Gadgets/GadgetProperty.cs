using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public enum GadgetProperty
{
    HatchGroupId,
    TeamId,
    SkillId,
    Width,
    Height,
    RawLemmingState,
    Count,
    InitialAnimationFrame,
    LogicGateType,
    IsFastForwards
}

public static class GadgetPropertyHelpers
{
    public static GadgetProperty GetGadgetProperty(int rawValue)
    {
        var enumValue = (GadgetProperty)rawValue;

        return enumValue switch
        {
            GadgetProperty.HatchGroupId => GadgetProperty.HatchGroupId,
            GadgetProperty.TeamId => GadgetProperty.TeamId,
            GadgetProperty.SkillId => GadgetProperty.SkillId,
            GadgetProperty.Width => GadgetProperty.Width,
            GadgetProperty.Height => GadgetProperty.Height,
            GadgetProperty.RawLemmingState => GadgetProperty.RawLemmingState,
            GadgetProperty.Count => GadgetProperty.Count,
            GadgetProperty.InitialAnimationFrame => GadgetProperty.InitialAnimationFrame,
            GadgetProperty.LogicGateType => GadgetProperty.LogicGateType,

            _ => Helpers.ThrowUnknownEnumValueException<GadgetProperty>(rawValue)
        };
    }
}

public readonly struct GadgetPropertyHasher : IPerfectHasher<GadgetProperty>, IBitBufferCreator<BitBuffer32>
{
    public int NumberOfItems => 10;

    [Pure]
    public int Hash(GadgetProperty item) => (int)item;
    [Pure]
    public GadgetProperty UnHash(int index) => (GadgetProperty)index;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArraySet<GadgetPropertyHasher, BitBuffer32, GadgetProperty> CreateBitArraySet(bool fullSet = false) => new(new GadgetPropertyHasher(), fullSet);
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, TValue> CreateBitArrayDictionary<TValue>() => new(new GadgetPropertyHasher());

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();
}