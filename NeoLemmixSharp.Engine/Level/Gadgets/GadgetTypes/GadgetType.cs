using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public abstract class GadgetType : IExtendedEnumType<GadgetType>
{
    private static readonly GadgetType[] GadgetTypes = RegisterAllGadgetTypes();

    public static int NumberOfItems => GadgetTypes.Length;
    public static ReadOnlySpan<GadgetType> AllItems => new(GadgetTypes);

    private static GadgetType[] RegisterAllGadgetTypes()
    {
        var result = new GadgetType[]
        {
            GenericGadgetType.Instance,
            WaterGadgetType.Instance,
            FireGadgetType.Instance,
            TinkerableGadgetType.Instance,
            UpdraftGadgetType.Instance,
            SplatGadgetType.Instance,
            NoSplatGadgetType.Instance,
            MetalGrateGadgetType.Instance,
            SwitchGadgetType.Instance,
            SawBladeGadgetType.Instance,
            FunctionalGadgetType.Instance,
            LogicGateGadgetType.Instance,
            HatchGadgetType.Instance
        };

        result.ValidateUniqueIds();
        Array.Sort(result, IdEquatableItemHelperMethods.Compare);

        return result;
    }

    public abstract int Id { get; }
    public abstract string GadgetTypeName { get; }

    public bool Equals(GadgetType? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetType other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => GadgetTypeName;

    public static bool operator ==(GadgetType left, GadgetType right) => left.Id == right.Id;
    public static bool operator !=(GadgetType left, GadgetType right) => left.Id != right.Id;
}