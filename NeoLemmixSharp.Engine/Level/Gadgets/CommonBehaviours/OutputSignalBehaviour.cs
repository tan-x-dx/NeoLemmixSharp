using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;
using NeoLemmixSharp.IO.Data.Style.Gadget.Behaviour;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

public sealed class OutputSignalBehaviour : GadgetBehaviour
{
    private readonly IGadgetLinkTrigger[] _gadgetLinkTriggers;

    public OutputSignalBehaviour(IGadgetLinkTrigger[] gadgetLinkTriggers)
        : base(GadgetBehaviourType.GadgetOutputSignal)
    {
        _gadgetLinkTriggers = gadgetLinkTriggers;
    }

    protected override void PerformInternalBehaviour(int lemmingId)
    {
        var signal = lemmingId >= 0;

        foreach (var trigger in _gadgetLinkTriggers)
        {
            trigger.ReactToSignal(signal);
        }
    }
}
