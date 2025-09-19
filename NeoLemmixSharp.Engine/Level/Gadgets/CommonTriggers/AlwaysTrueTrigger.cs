using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class AlwaysTrueTrigger : GadgetTrigger
{
    public AlwaysTrueTrigger()
        : base(GadgetTriggerType.AlwaysTrue)
    {
    }

    public override void DetectTrigger()
    {
        DetermineTrigger(true);
        TriggerBehaviours();
        MarkAsEvaluated();
    }
}
