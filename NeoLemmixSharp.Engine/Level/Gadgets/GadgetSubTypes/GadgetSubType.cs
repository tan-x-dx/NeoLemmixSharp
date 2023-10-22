using NeoLemmixSharp.Common.Util.Identity;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;

public abstract class GadgetSubType : IExtendedEnumType<GadgetSubType>
{
    private static readonly GadgetSubType[] GadgetTypes = RegisterAllGadgetTypes();

    public static int NumberOfItems => GadgetTypes.Length;
    public static ReadOnlySpan<GadgetSubType> AllItems => new(GadgetTypes);

    private static GadgetSubType[] RegisterAllGadgetTypes()
    {
        var result = new GadgetSubType[]
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

    public bool Equals(GadgetSubType? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetSubType other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => GadgetTypeName;

    public static bool operator ==(GadgetSubType left, GadgetSubType right) => left.Id == right.Id;
    public static bool operator !=(GadgetSubType left, GadgetSubType right) => left.Id != right.Id;
}