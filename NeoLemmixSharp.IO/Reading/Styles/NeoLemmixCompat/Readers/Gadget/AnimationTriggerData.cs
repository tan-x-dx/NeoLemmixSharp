namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

internal sealed class AnimationTriggerData
{
    public NeoLemmixGadgetCondition Condition { get; internal set; }
    public NeoLemmixStateType StateType { get; internal set; }
    public bool Hide { get; internal set; }
}
