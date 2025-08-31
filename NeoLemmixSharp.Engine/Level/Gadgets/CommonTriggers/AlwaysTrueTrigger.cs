using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class AlwaysTrueTrigger : GadgetTrigger
{
    public AlwaysTrueTrigger() : base(GadgetTriggerType.AlwaysTrue)
    {
    }

    public override void Tick()
    {
        foreach (var behaviour in Behaviours)
        {
            RegisterCauseAndEffectData(behaviour.Id);
        }
    }
}
