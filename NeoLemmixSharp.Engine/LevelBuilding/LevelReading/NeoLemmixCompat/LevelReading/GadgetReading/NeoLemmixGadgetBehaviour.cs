using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

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
        _ => throw new ArgumentOutOfRangeException(nameof(neoLemmixGadgetBehaviour), neoLemmixGadgetBehaviour, null)
    };
}