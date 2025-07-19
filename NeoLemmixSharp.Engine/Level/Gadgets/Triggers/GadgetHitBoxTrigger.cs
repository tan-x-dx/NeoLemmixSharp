using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Triggers;

public sealed class GadgetHitBoxTrigger : GadgetTrigger
{
    private readonly GadgetBehaviour[] _behaviours;

    public GadgetHitBoxTrigger(
        GadgetTriggerName triggerName,
        GadgetBehaviour[] behaviours)
        : base(triggerName)
    {
        _behaviours = behaviours;
    }


}
