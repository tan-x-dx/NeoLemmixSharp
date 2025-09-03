using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class AlwaysTrueTrigger : GadgetTrigger
{
    private readonly GadgetBehaviour[] _behaviours;

    public AlwaysTrueTrigger(GadgetBehaviour[] behaviours)
        : base(GadgetTriggerType.AlwaysTrue)
    {
        _behaviours = behaviours;
    }

    public override ReadOnlySpan<GadgetBehaviour> Behaviours => new(_behaviours);

    public override void Tick()
    {
        foreach (var behaviour in _behaviours)
        {
            RegisterCauseAndEffectData(behaviour.Id);
        }
    }
}
