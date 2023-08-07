using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Collections;

public interface IGadgetCollection<TGadget>
    where TGadget : class, IHitBoxGadget
{
    [Pure]
    bool TryGetGadgetThatMatchesTypeAndOrientation(LevelPosition levelPosition, Orientation orientation, [NotNullWhen(true)] out TGadget? gadget);
}

public static class GadgetCollections
{
    public static IGadgetCollection<IHitBoxGadget> GeneralGadgets { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> Waters { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<IHitBoxGadget> Updrafts { get; private set; } = EmptyGadgetList<IHitBoxGadget>.Instance;
    public static IGadgetCollection<MetalGrateGadget> MetalGrates { get; private set; } = EmptyGadgetList<MetalGrateGadget>.Instance;

    public static void ClearGadgets()
    {
        Waters = EmptyGadgetList<IHitBoxGadget>.Instance;
        Updrafts = EmptyGadgetList<IHitBoxGadget>.Instance;
        GeneralGadgets = EmptyGadgetList<IHitBoxGadget>.Instance;
        MetalGrates = EmptyGadgetList<MetalGrateGadget>.Instance;
    }

    public static void SetGadgets(IEnumerable<IGadget> allGadgets)
    {
        var gadgetTypeLookup = allGadgets
            .OfType<IHitBoxGadget>()
            .ToLookup(g => g.Type);

        Waters = GetGadgetCollection<IHitBoxGadget>(GadgetType.Water);
        Updrafts = GetGadgetCollection<IHitBoxGadget>(GadgetType.Updraft);
        GeneralGadgets = GetGadgetCollection<IHitBoxGadget>(GadgetType.TrapOnce);

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