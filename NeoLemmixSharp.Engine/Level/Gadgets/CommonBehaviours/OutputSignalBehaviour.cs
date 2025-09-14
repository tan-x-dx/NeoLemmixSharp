using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

public sealed class OutputSignalBehaviour : GadgetBehaviour
{
    private IGadgetLinkTrigger[] _gadgetLinkTriggers = [];

    public OutputSignalBehaviour()
        : base(GadgetBehaviourType.GadgetOutputSignal)
    {
    }

    public void SetGadgetLinkTriggers(IGadgetLinkTrigger[] gadgetLinkTriggers) => _gadgetLinkTriggers = gadgetLinkTriggers;

    protected override void PerformInternalBehaviour(int _)
    {
        foreach (var trigger in _gadgetLinkTriggers)
        {
            trigger.ReactToSignal();
        }
    }
}
