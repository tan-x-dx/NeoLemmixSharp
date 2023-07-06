using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public interface IGadgetCollection<TGadget>
    where TGadget : class, IGadget
{
    bool TryGetGadgetThatMatchesTypeAndOrientation(LevelPosition levelPosition, Orientation orientation, out TGadget? gadget);
}

public static class GadgetCollections
{
    public static IGadgetCollection<IGadget> Hatches { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> Exits { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> Waters { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> InfiniteTraps { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> Fires { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> Updrafts { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> OneTimeTraps { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> ForceSplats { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> NoSplats { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<MetalGrateGadget> MetalGrates { get; private set; } = EmptyGadgetList<MetalGrateGadget>.Instance;

    public static void ClearGadgets()
    {
        Hatches = EmptyGadgetList<IGadget>.Instance;
        Exits = EmptyGadgetList<IGadget>.Instance;
        Waters = EmptyGadgetList<IGadget>.Instance;
        InfiniteTraps = EmptyGadgetList<IGadget>.Instance;
        Fires = EmptyGadgetList<IGadget>.Instance;
        Updrafts = EmptyGadgetList<IGadget>.Instance;
        OneTimeTraps = EmptyGadgetList<IGadget>.Instance;
        ForceSplats = EmptyGadgetList<IGadget>.Instance;
        NoSplats = EmptyGadgetList<IGadget>.Instance;
        MetalGrates = EmptyGadgetList<MetalGrateGadget>.Instance;
    }

    public static void SetGadgets(IEnumerable<IGadget> allGadgets)
    {
        var gadgetTypeLookup = allGadgets
            .ToLookup(g => g.Type);

        Hatches = GetGadgetCollection<IGadget>(GadgetType.Hatch);
        Exits = GetGadgetCollection<IGadget>(GadgetType.Exit);
        Waters = GetGadgetCollection<IGadget>(GadgetType.Water);
        InfiniteTraps = GetGadgetCollection<IGadget>(GadgetType.TrapInfinite);
        Fires = GetGadgetCollection<IGadget>(GadgetType.Fire);
        Updrafts = GetGadgetCollection<IGadget>(GadgetType.Updraft);
        OneTimeTraps = GetGadgetCollection<IGadget>(GadgetType.TrapOnce);
        NoSplats = GetGadgetCollection<IGadget>(GadgetType.NoSplat);
        ForceSplats = GetGadgetCollection<IGadget>(GadgetType.Splat);

        MetalGrates = GetGadgetCollection<MetalGrateGadget>(GadgetType.MetalGrate);

        IGadgetCollection<TGadget> GetGadgetCollection<TGadget>(GadgetType gadgetType)
            where TGadget : class, IGadget
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