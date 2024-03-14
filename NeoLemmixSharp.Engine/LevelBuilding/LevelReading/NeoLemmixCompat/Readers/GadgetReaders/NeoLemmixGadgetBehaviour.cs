using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public enum NeoLemmixGadgetBehaviour
{
    None,
    Entrance,
    Exit,
    Water,
    Fire,
    OneWayRight,
    OneWayUp,
    OneWayLeft,
    OneWayDown,
    PickupSkill,
    LockedExit,
    UnlockButton,
    ForceLeft,
    ForceRight,
    Trap,
    TrapOnce,
    Teleporter,
    Receiver,
    Updraft,
    Splitter,
    AntiSplatPad,
    SplatPad,
    Background
}

public static class NeoLemmixGadgetBehaviourExtensions
{
    public static GadgetBehaviour? ToGadgetBehaviour(
        this NeoLemmixGadgetBehaviour neoLemmixGadgetBehaviour) => neoLemmixGadgetBehaviour switch
    {
        NeoLemmixGadgetBehaviour.None => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Entrance => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Exit => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Water => WaterGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Fire => FireGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.OneWayRight => null,
        NeoLemmixGadgetBehaviour.OneWayUp => null,
        NeoLemmixGadgetBehaviour.OneWayLeft => null,
        NeoLemmixGadgetBehaviour.OneWayDown => null,
        NeoLemmixGadgetBehaviour.PickupSkill => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.LockedExit => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.UnlockButton => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.ForceLeft => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.ForceRight => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Trap => TinkerableGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.TrapOnce => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Teleporter => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Receiver => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Updraft => UpdraftGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Splitter => GenericGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.AntiSplatPad => NoSplatGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.SplatPad => SplatGadgetBehaviour.Instance,
        NeoLemmixGadgetBehaviour.Background => GenericGadgetBehaviour.Instance,

        _ => ThrowUnknownBehaviourException<GadgetBehaviour?>(neoLemmixGadgetBehaviour)
    };

    public static int GetNumberOfExtraStates(
        this NeoLemmixGadgetBehaviour neoLemmixGadgetBehaviour) => neoLemmixGadgetBehaviour switch
    {
        NeoLemmixGadgetBehaviour.None => 0,
        NeoLemmixGadgetBehaviour.Entrance => 0,
        NeoLemmixGadgetBehaviour.Exit => 0,
        NeoLemmixGadgetBehaviour.Water => 0,
        NeoLemmixGadgetBehaviour.Fire => 0,
        NeoLemmixGadgetBehaviour.OneWayRight => 0,
        NeoLemmixGadgetBehaviour.OneWayUp => 0,
        NeoLemmixGadgetBehaviour.OneWayLeft => 0,
        NeoLemmixGadgetBehaviour.OneWayDown => 0,
        NeoLemmixGadgetBehaviour.PickupSkill => 1,
        NeoLemmixGadgetBehaviour.LockedExit => 1,
        NeoLemmixGadgetBehaviour.UnlockButton => 1,
        NeoLemmixGadgetBehaviour.ForceLeft => 0,
        NeoLemmixGadgetBehaviour.ForceRight => 0,
        NeoLemmixGadgetBehaviour.Trap => 2,
        NeoLemmixGadgetBehaviour.TrapOnce => 1,
        NeoLemmixGadgetBehaviour.Teleporter => 1,
        NeoLemmixGadgetBehaviour.Receiver => 1,
        NeoLemmixGadgetBehaviour.Updraft => 0,
        NeoLemmixGadgetBehaviour.Splitter => 1,
        NeoLemmixGadgetBehaviour.AntiSplatPad => 0,
        NeoLemmixGadgetBehaviour.SplatPad => 0,
        NeoLemmixGadgetBehaviour.Background => 0,

        _ => ThrowUnknownBehaviourException<int>(neoLemmixGadgetBehaviour)
    };

    [DoesNotReturn]
    private static T ThrowUnknownBehaviourException<T>(NeoLemmixGadgetBehaviour neoLemmixGadgetBehaviour)
    {
        throw new ArgumentOutOfRangeException(
            nameof(neoLemmixGadgetBehaviour),
            neoLemmixGadgetBehaviour,
            "Unknown NeoLemmix gadget behaviour");
    }
}