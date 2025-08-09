using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.IO.Data.Level.Gadgets;

public sealed class GadgetStateInstanceData
{
    public required GadgetStateName CustomStateName { get; init; }
    public required GadgetTriggerData[] CustomTriggers { get; init; }
    public required GadgetBehaviourData[] CustomBehaviours { get; init; }
    public required HitBoxCriteriaData[] CustomHitBoxCriteria { get; init; }

}
