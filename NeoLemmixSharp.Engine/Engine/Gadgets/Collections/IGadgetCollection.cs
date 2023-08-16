using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets.MetalGrates;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Collections;

public interface IGadgetCollection<TGadget>
    where TGadget : class, IGadget
{
    [Pure]
    bool TryGetGadgetThatMatchesTypeAndOrientation(Lemming lemming, LevelPosition levelPosition, [NotNullWhen(true)] out TGadget? gadget);
}

public static class GadgetCollections
{
    public static IGadgetCollection<IGadget> GeneralGadgets { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> Waters { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<IGadget> Updrafts { get; private set; } = EmptyGadgetList<IGadget>.Instance;
    public static IGadgetCollection<MetalGrateGadget> MetalGrates { get; private set; } = EmptyGadgetList<MetalGrateGadget>.Instance;

    public static void ClearGadgets()
    {
        Waters = EmptyGadgetList<IGadget>.Instance;
        Updrafts = EmptyGadgetList<IGadget>.Instance;
        GeneralGadgets = EmptyGadgetList<IGadget>.Instance;
        MetalGrates = EmptyGadgetList<MetalGrateGadget>.Instance;
    }

    public static void SetGadgets(IEnumerable<IGadget> allGadgets)
    {
        var gadgetTypeLookup = allGadgets
            .ToLookup(g => g.Type);

        Waters = GetGadgetCollection<IGadget>(GadgetType.Water);
        Updrafts = GetGadgetCollection<IGadget>(GadgetType.Updraft);
        GeneralGadgets = GetGadgetCollection<IGadget>(GadgetType.None);

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