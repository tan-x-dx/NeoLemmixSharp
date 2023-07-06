using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public interface IGadgetCollection
{
    bool TryGetGadgetThatMatchesTypeAndOrientation(LevelPosition levelPosition, Orientation orientation, out IGadget? gadget);
}

public sealed class GadgetList : IGadgetCollection
{
    private readonly IGadget[] _gadgets;

    public GadgetList(IGadget[] gadgets)
    {
        _gadgets = gadgets;
    }

    public bool TryGetGadgetThatMatchesTypeAndOrientation(
        LevelPosition levelPosition,
        Orientation orientation,
        out IGadget? gadget)
    {
        for (var i = 0; i < _gadgets.Length; i++)
        {
            gadget = _gadgets[i];
            if (gadget.MatchesOrientation(levelPosition, orientation))
                return true;
        }

        gadget = null;
        return false;
    }
}

public sealed class EmptyGadgetList : IGadgetCollection
{
    public static EmptyGadgetList Instance { get; } = new();

    private EmptyGadgetList()
    {
    }

    public bool TryGetGadgetThatMatchesTypeAndOrientation(
        LevelPosition levelPosition,
        Orientation orientation,
        out IGadget? gadget)
    {
        gadget = null;
        return false;
    }
}

public static class GadgetCollections
{
    public static IGadgetCollection Hatches { get; private set; }
    public static IGadgetCollection Exits { get; private set; }
    public static IGadgetCollection Waters { get; private set; }
    public static IGadgetCollection InfiniteTraps { get; private set; }
    public static IGadgetCollection Fires { get; private set; }
    public static IGadgetCollection Updrafts { get; private set; }
    public static IGadgetCollection OneTimeTraps { get; private set; }
    public static IGadgetCollection ForceSplats { get; private set; }
    public static IGadgetCollection NoSplats { get; private set; }
    public static IGadgetCollection MetalGrates { get; private set; }

    public static void ClearGadgets()
    {
        Hatches = EmptyGadgetList.Instance;
        Exits = EmptyGadgetList.Instance;
        Waters = EmptyGadgetList.Instance;
        InfiniteTraps = EmptyGadgetList.Instance;
        Fires = EmptyGadgetList.Instance;
        Updrafts = EmptyGadgetList.Instance;
        OneTimeTraps = EmptyGadgetList.Instance;
        ForceSplats = EmptyGadgetList.Instance;
        NoSplats = EmptyGadgetList.Instance;
        MetalGrates = EmptyGadgetList.Instance;
    }

    public static void SetGadgets(IEnumerable<IGadget> allGadgets)
    {
        var gadgetTypeLookup = allGadgets
            .ToLookup(g => g.Type);

        var hatches = gadgetTypeLookup[GadgetType.Hatch].ToArray();
        Hatches = hatches.Length > 0
            ? new GadgetList(hatches)
            : EmptyGadgetList.Instance;

        var exits = gadgetTypeLookup[GadgetType.Exit].ToArray();
        Exits = exits.Length > 0
            ? new GadgetList(exits)
            : EmptyGadgetList.Instance;

        var waters = gadgetTypeLookup[GadgetType.Water].ToArray();
        Waters = waters.Length > 0
            ? new GadgetList(waters)
            : EmptyGadgetList.Instance;

        var infiniteTraps = gadgetTypeLookup[GadgetType.TrapInfinite].ToArray();
        InfiniteTraps = infiniteTraps.Length > 0
            ? new GadgetList(infiniteTraps)
            : EmptyGadgetList.Instance;

        var fires = gadgetTypeLookup[GadgetType.Fire].ToArray();
        Fires = fires.Length > 0
            ? new GadgetList(fires)
            : EmptyGadgetList.Instance;

        var updrafts = gadgetTypeLookup[GadgetType.Updraft].ToArray();
        Updrafts = updrafts.Length > 0
            ? new GadgetList(updrafts)
            : EmptyGadgetList.Instance;

        var oneTimeTraps = gadgetTypeLookup[GadgetType.TrapOnce].ToArray();
        OneTimeTraps = oneTimeTraps.Length > 0
            ? new GadgetList(oneTimeTraps)
            : EmptyGadgetList.Instance;

        var noSplats = gadgetTypeLookup[GadgetType.NoSplat].ToArray();
        NoSplats = noSplats.Length > 0
            ? new GadgetList(noSplats)
            : EmptyGadgetList.Instance;

        var splats = gadgetTypeLookup[GadgetType.Splat].ToArray();
        ForceSplats = splats.Length > 0
            ? new GadgetList(splats)
            : EmptyGadgetList.Instance;

        var metalGrates = gadgetTypeLookup[GadgetType.MetalGrate]
            .Cast<MetalGrateGadget>()
            .ToArray();
        MetalGrates = metalGrates.Length > 0
            ? new GadgetList(metalGrates)
            : EmptyGadgetList.Instance;
    }
}