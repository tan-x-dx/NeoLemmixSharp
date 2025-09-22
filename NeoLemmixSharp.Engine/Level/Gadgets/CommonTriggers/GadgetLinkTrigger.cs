using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class GadgetLinkTrigger : GadgetTrigger, IGadgetLinkTrigger
{
    public GadgetLinkTrigger()
        : base(GadgetTriggerType.GadgetLinkTrigger)
    {
    }

    public override void DetectTrigger()
    {
        // This type is at the mercy of its (not-null) input signal.
    }

    public void ReactToSignal(int payload)
    {
        DetermineTrigger(true);
        TriggerBehaviours();
        MarkAsEvaluated();
    }
}
