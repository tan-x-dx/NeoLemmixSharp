using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public interface IGadgetCollection<TGadget>
    where TGadget : class, IHitBoxGadget
{
    [Pure]
    bool TryGetGadgetThatMatchesTypeAndOrientation(LevelPosition levelPosition, Orientation orientation, [NotNullWhen(true)] out TGadget? gadget);
}

public static class GadgetCollections
{
    public static IGadgetCollection<IHitBoxGadget> Hatches { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> Exits { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> Waters { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> InfiniteTraps { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> Fires { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> Updrafts { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> OneTimeTraps { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> ForceSplats { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> NoSplats { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<MetalGrateGadget> MetalGrates { get; private set; } = EmptyGadgetList<MetalGrateGadget>.Instance;

    public static void ClearGadgets()
    {
        Hatches = EmptyGadgetList<IHitBoxGadget>.Instance;
        Exits = EmptyGadgetList<IHitBoxGadget>.Instance;
        Waters = EmptyGadgetList<IHitBoxGadget>.Instance;
        InfiniteTraps = EmptyGadgetList<IHitBoxGadget>.Instance;
        Fires = EmptyGadgetList<IHitBoxGadget>.Instance;
        Updrafts = EmptyGadgetList<IHitBoxGadget>.Instance;
        OneTimeTraps = EmptyGadgetList<IHitBoxGadget>.Instance;
        ForceSplats = EmptyGadgetList<IHitBoxGadget>.Instance;
        NoSplats = EmptyGadgetList<IHitBoxGadget>.Instance;
        MetalGrates = EmptyGadgetList<MetalGrateGadget>.Instance;
    }

    public static void SetGadgets(IEnumerable<IGadget> allGadgets)
    {
        var gadgetTypeLookup = allGadgets
            .OfType<IHitBoxGadget>()
            .ToLookup(g => g.Type);

        Hatches = GetGadgetCollection<IHitBoxGadget>(GadgetType.Hatch);
        Exits = GetGadgetCollection<IHitBoxGadget>(GadgetType.Exit);
        Waters = GetGadgetCollection<IHitBoxGadget>(GadgetType.Water);
        InfiniteTraps = GetGadgetCollection<IHitBoxGadget>(GadgetType.TrapInfinite);
        Fires = GetGadgetCollection<IHitBoxGadget>(GadgetType.Fire);
        Updrafts = GetGadgetCollection<IHitBoxGadget>(GadgetType.Updraft);
        OneTimeTraps = GetGadgetCollection<IHitBoxGadget>(GadgetType.TrapOnce);
        NoSplats = GetGadgetCollection<IHitBoxGadget>(GadgetType.NoSplat);
        ForceSplats = GetGadgetCollection<IHitBoxGadget>(GadgetType.Splat);

        MetalGrates = GetGadgetCollection<MetalGrateGadget>(GadgetType.MetalGrate);

        IGadgetCollection<TGadget> GetGadgetCollection<TGadget>(GadgetType gadgetType)
            where TGadget : class, IHitBoxGadget
        {
            var relevantGadgets = gadgetTypeLookup[gadgetType]
                .OrderBy(g => g.Id)
                .Cast<TGadget>()
                .ToArray();

            return relevantGadgets.Length > 0
                ? new SimpleGadgetList<TGadget>(relevantGadgets)
                : EmptyGadgetList<TGadget>.Instance;
        }
    }
}