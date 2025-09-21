using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

public sealed class OutputSignalBehaviour : GadgetBehaviour
{
    private LinkData[] _gadgetLinkData = [];

    public OutputSignalBehaviour()
        : base(GadgetBehaviourType.GadgetOutputSignal)
    {
    }

    public void SetGadgetLinkTriggers(LinkData[] gadgetLinkData) => _gadgetLinkData = gadgetLinkData;

    protected override void PerformInternalBehaviour(int _)
    {
        foreach (var gadgetLinkDatum in _gadgetLinkData)
        {
            gadgetLinkDatum.GadgetLinkTrigger.ReactToSignal(gadgetLinkDatum.Payload);
        }
    }
}

public readonly record struct LinkData(IGadgetLinkTrigger GadgetLinkTrigger, int Payload);
