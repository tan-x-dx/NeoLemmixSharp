using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class GadgetLinkTrigger : GadgetTrigger, IGadgetLinkTrigger
{
    public OutputSignalBehaviour InputSignalBehaviour { get; set; }

    private readonly GadgetBehaviour[] _behaviours;

    public GadgetLinkTrigger(GadgetBehaviour[] behaviours) : base(GadgetTriggerType.GadgetLinkTrigger)
    {
        _behaviours = behaviours;
    }

    public override void DetectTrigger(GadgetBase parentGadget)
    {
        throw new NotImplementedException();
    }

    public void ReactToSignal(bool signal)
    {
        DetermineTrigger(signal);
        LevelScreen.CauseAndEffectManager.MarkTriggerAsEvaluated(this);
    }
}
