using NeoLemmixSharp.Engine.Gadgets.Types;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Util;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Engine.Gadgets;

public abstract class GadgetCollection<TGadget>
    where TGadget : Gadget
{
    public abstract bool TryGetGadgetWherePixelIsSolidToLemming(LevelPosition levelPosition, Lemming lemming, out TGadget? gadget);
    public abstract bool TryGetGadgetWherePixelIsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming, out TGadget? gadget);
    public abstract bool TryGetGadgetThatMatchesTypeAndOrientation(LevelPosition levelPosition, Orientation orientation, out TGadget? gadget);
}

public sealed class GadgetList<TGadget> : GadgetCollection<TGadget>
    where TGadget : Gadget
{
    private readonly TGadget[] _gadgets;

    public GadgetList(TGadget[] gadgets)
    {
        _gadgets = gadgets;
    }

    public override bool TryGetGadgetWherePixelIsSolidToLemming(
        LevelPosition levelPosition,
        Lemming lemming,
        out TGadget? gadget)
    {
        for (var i = 0; i < _gadgets.Length; i++)
        {
            gadget = _gadgets[i];
            if (gadget.IsSolidToLemming(levelPosition, lemming))
                return true;
        }

        gadget = null;
        return false;
    }

    public override bool TryGetGadgetWherePixelIsIndestructibleToLemming(
        LevelPosition levelPosition,
        Lemming lemming,
        out TGadget? gadget)
    {
        for (var i = 0; i < _gadgets.Length; i++)
        {
            gadget = _gadgets[i];
            if (gadget.IsIndestructibleToLemming(levelPosition, lemming))
                return true;
        }

        gadget = null;
        return false;
    }

    public override bool TryGetGadgetThatMatchesTypeAndOrientation(
        LevelPosition levelPosition,
        Orientation orientation,
        out TGadget? gadget)
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

public sealed class EmptyGadgetList<TGadget> : GadgetCollection<TGadget>
    where TGadget : Gadget
{
    public static EmptyGadgetList<TGadget> Instance { get; } = new();

    private EmptyGadgetList()
    {
    }

    public override bool TryGetGadgetWherePixelIsSolidToLemming(
        LevelPosition levelPosition,
        Lemming lemming,
        out TGadget? gadget)
    {
        gadget = null;
        return false;
    }

    public override bool TryGetGadgetWherePixelIsIndestructibleToLemming(
        LevelPosition levelPosition,
        Lemming lemming,
        out TGadget? gadget)
    {
        gadget = null;
        return false;
    }

    public override bool TryGetGadgetThatMatchesTypeAndOrientation(
        LevelPosition levelPosition,
        Orientation orientation,
        out TGadget? gadget)
    {
        gadget = null;
        return false;
    }
}

public static class GadgetCollections
{
    public static GadgetCollection<HatchGadget> Hatches { get; private set; }
    public static GadgetCollection<ExitGadget> Exits { get; private set; }
    public static GadgetCollection<WaterGadget> Waters { get; private set; }
    public static GadgetCollection<TrapInfiniteGadget> InfiniteTraps { get; private set; }
    public static GadgetCollection<FireGadget> Fires { get; private set; }
    public static GadgetCollection<UpdraftGadget> Updrafts { get; private set; }
    public static GadgetCollection<TrapOnceGadget> OneTimeTraps { get; private set; }
    public static GadgetCollection<SplatGadget> ForceSplats { get; private set; }
    public static GadgetCollection<NoSplatGadget> NoSplats { get; private set; }

    public static void ClearGadgets()
    {
        Hatches = EmptyGadgetList<HatchGadget>.Instance;
        Exits = EmptyGadgetList<ExitGadget>.Instance;
        Waters = EmptyGadgetList<WaterGadget>.Instance;
        InfiniteTraps = EmptyGadgetList<TrapInfiniteGadget>.Instance;
        Fires = EmptyGadgetList<FireGadget>.Instance;
        Updrafts = EmptyGadgetList<UpdraftGadget>.Instance;
        OneTimeTraps = EmptyGadgetList<TrapOnceGadget>.Instance;
        ForceSplats = EmptyGadgetList<SplatGadget>.Instance;
        NoSplats = EmptyGadgetList<NoSplatGadget>.Instance;
    }

    public static void SetGadgets(ICollection<Gadget> allGadgets)
    {
        var hatches = allGadgets.OfType<HatchGadget>().ToArray();
        Hatches = hatches.Length > 0
            ? new GadgetList<HatchGadget>(hatches)
            : EmptyGadgetList<HatchGadget>.Instance;

        var exits = allGadgets.OfType<ExitGadget>().ToArray();
        Exits = exits.Length > 0
            ? new GadgetList<ExitGadget>(exits)
            : EmptyGadgetList<ExitGadget>.Instance;

        var waters = allGadgets.OfType<WaterGadget>().ToArray();
        Waters = waters.Length > 0
            ? new GadgetList<WaterGadget>(waters)
            : EmptyGadgetList<WaterGadget>.Instance;

        var infiniteTraps = allGadgets.OfType<TrapInfiniteGadget>().ToArray();
        InfiniteTraps = infiniteTraps.Length > 0
            ? new GadgetList<TrapInfiniteGadget>(infiniteTraps)
            : EmptyGadgetList<TrapInfiniteGadget>.Instance;

        var fires = allGadgets.OfType<FireGadget>().ToArray();
        Fires = fires.Length > 0
            ? new GadgetList<FireGadget>(fires)
            : EmptyGadgetList<FireGadget>.Instance;

        var updrafts = allGadgets.OfType<UpdraftGadget>().ToArray();
        Updrafts = updrafts.Length > 0
            ? new GadgetList<UpdraftGadget>(updrafts)
            : EmptyGadgetList<UpdraftGadget>.Instance;

        var oneTimeTraps = allGadgets.OfType<TrapOnceGadget>().ToArray();
        OneTimeTraps = oneTimeTraps.Length > 0
            ? new GadgetList<TrapOnceGadget>(oneTimeTraps)
            : EmptyGadgetList<TrapOnceGadget>.Instance;

        var noSplats = allGadgets.OfType<NoSplatGadget>().ToArray();
        NoSplats = noSplats.Length > 0
            ? new GadgetList<NoSplatGadget>(noSplats)
            : EmptyGadgetList<NoSplatGadget>.Instance;

        var splats = allGadgets.OfType<SplatGadget>().ToArray();
        ForceSplats = splats.Length > 0
            ? new GadgetList<SplatGadget>(splats)
            : EmptyGadgetList<SplatGadget>.Instance;
    }
}