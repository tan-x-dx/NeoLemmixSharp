using NeoLemmixSharp.Common.Util.Identity;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public abstract class GadgetBehaviour : IExtendedEnumType<GadgetBehaviour>
{
    private static readonly GadgetBehaviour[] GadgetTypes = RegisterAllGadgetTypes();

    public static int NumberOfItems => GadgetTypes.Length;
    public static ReadOnlySpan<GadgetBehaviour> AllItems => new(GadgetTypes);

    private static GadgetBehaviour[] RegisterAllGadgetTypes()
    {
        var result = new GadgetBehaviour[]
        {
            GenericGadgetBehaviour.Instance,
            WaterGadgetBehaviour.Instance,
            FireGadgetBehaviour.Instance,
            TinkerableGadgetBehaviour.Instance,
            UpdraftGadgetBehaviour.Instance,
            SplatGadgetBehaviour.Instance,
            NoSplatGadgetBehaviour.Instance,
            MetalGrateGadgetBehaviour.Instance,
            SwitchGadgetBehaviour.Instance,
            SawBladeGadgetBehaviour.Instance,
            FunctionalGadgetBehaviour.Instance,
            LogicGateGadgetBehaviour.Instance,
            HatchGadgetBehaviour.Instance
        };

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<GadgetBehaviour>(result));
        Array.Sort(result, IdEquatableItemHelperMethods.Compare);

        return result;
    }

    public abstract int Id { get; }
    public abstract string GadgetTypeName { get; }

    public bool Equals(GadgetBehaviour? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBehaviour other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => GadgetTypeName;

    public static bool operator ==(GadgetBehaviour left, GadgetBehaviour right) => left.Id == right.Id;
    public static bool operator !=(GadgetBehaviour left, GadgetBehaviour right) => left.Id != right.Id;
}