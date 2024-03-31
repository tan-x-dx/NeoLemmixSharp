using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

public sealed class AnimationTriggerData
{
    public NeoLemmixGadgetStateType StateType { get; set; }
    public GadgetSecondaryAnimationAction AnimationAction { get; set; }
    public bool Hide { get; set; }
}

public enum NeoLemmixGadgetStateType
{
    Idle,
    Active,
    Disabled
}