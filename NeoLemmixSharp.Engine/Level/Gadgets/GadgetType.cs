namespace NeoLemmixSharp.Engine.Level.Gadgets;

public enum GadgetType
{
    Generic,
    Hatch,
    Exit,
    Water,
    Fire,

    TrapInfinite,
    TrapOnce,

    Button,
    SkillPickup,

    ForceDirection,
    OneWayArrow,

    BlockerForceDirectionArea,
    BlockerArea,
    Teleport,

    SawBlade,
    Updraft,
    Flipper,
    NoSplat,
    Splat,
    Zombie,

    Switch,
    Mover,

    MetalGrateOff,
    MetalGrateActivating,
    MetalGrateOn,
    MetalGrateDeactivating,

    AndGate,
    OrGate,
    NotGate,
    XorGate
}