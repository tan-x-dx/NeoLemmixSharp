using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class GadgetLinkTrigger : GadgetTrigger, IGadgetLinkTrigger
{
    public OutputSignalBehaviour? InputSignalBehaviour { get; set; }


    public GadgetLinkTrigger()
        : base(GadgetTriggerType.GadgetLinkTrigger)
    {
    }

    public override void DetectTrigger()
    {
        if (InputSignalBehaviour is null)
        {
            DetermineTrigger(false);
            MarkAsEvaluated();
        }

        // Otherwise do nothing.
        // This type is at the mercy of its (not-null) input signal.
    }

    public void ReactToSignal()
    {
        DetermineTrigger(true);
        TriggerBehaviours();
        MarkAsEvaluated();
    }
}
