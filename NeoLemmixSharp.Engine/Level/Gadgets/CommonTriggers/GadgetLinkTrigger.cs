using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class GadgetLinkTrigger : GadgetTrigger, IGadgetLinkTrigger
{
    private readonly GadgetBehaviour[] _behaviours;

    public GadgetLinkTrigger(GadgetBehaviour[] behaviours) : base(GadgetTriggerType.GadgetLinkTrigger)
    {
        _behaviours = behaviours;
    }

    public override ReadOnlySpan<GadgetBehaviour> Behaviours => new(_behaviours);

    public override void Tick()
    {
        throw new NotImplementedException();
    }

    public void ReactToSignal(bool signal)
    {
        throw new NotImplementedException();
    }
}
