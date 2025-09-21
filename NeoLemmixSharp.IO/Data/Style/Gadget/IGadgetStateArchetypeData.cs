using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public interface IGadgetStateArchetypeData
{
    GadgetStateName StateName { get; }

    ReadOnlySpan<GadgetTriggerData> InnateTriggers { get; }
    ReadOnlySpan<GadgetBehaviourData> InnateBehaviours { get; }
    ReadOnlySpan<GadgetTriggerBehaviourLink> TriggerBehaviourLinks { get; }
}

public readonly struct TriggerDataYouAreAllowedToModify
{
    public readonly GadgetTriggerName GadgetTriggerName;
    public readonly GadgetTriggerPropertyName GadgetTriggerPropertyName;
}

public readonly record struct GadgetTriggerPropertyName(string PropertyName);





// instance data crap below


public readonly struct TriggerDataIHaveModified
{
    public readonly GadgetTriggerName GadgetTriggerName;
    public readonly GadgetTriggerPropertyName GadgetTriggerPropertyName;
    public readonly int Data;
}





