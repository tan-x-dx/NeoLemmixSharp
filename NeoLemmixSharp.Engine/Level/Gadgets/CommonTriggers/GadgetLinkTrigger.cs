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

    public override void DetectTrigger(GadgetBase parentGadget)
    {
        // Do nothing. This type is at the mercy of its input signal
    }

    public void ReactToSignal()
    {
        DetermineTrigger(true);
        TriggerBehaviours();
        MarkAsEvaluated();
    }
}
