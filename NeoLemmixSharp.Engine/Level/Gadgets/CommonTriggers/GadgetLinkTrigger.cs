using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class GadgetLinkTrigger : GadgetTrigger, IGadgetLinkTrigger
{
    private readonly GadgetBehaviour[] _behaviours;

    public GadgetLinkTrigger(GadgetBehaviour[] behaviours) : base(GadgetTriggerType.GadgetLinkTrigger)
    {
        _behaviours = behaviours;
    }

    public override void DetectTrigger()
    {
        throw new NotImplementedException();
    }

    public void ReactToSignal(bool signal)
    {
        throw new NotImplementedException();
    }
}
