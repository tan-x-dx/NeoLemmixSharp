using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class AlwaysTrueTrigger : GadgetTrigger
{
    private readonly GadgetBehaviour[] _behaviours;

    public AlwaysTrueTrigger(GadgetBehaviour[] behaviours)
        : base(GadgetTriggerType.AlwaysTrue)
    {
        _behaviours = behaviours;
    }

    public override void DetectTrigger(GadgetBase parentGadget)
    {
        DetermineTrigger(true);
        LevelScreen.CauseAndEffectManager.MarkTriggerAsEvaluated(this);

        foreach (var behaviour in _behaviours)
        {
            RegisterCauseAndEffectData(behaviour.Id);
        }
    }
}
