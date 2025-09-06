using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetTrigger : IIdEquatable<GadgetTrigger>
{
    public required GadgetTriggerName TriggerName { get; init; }
    public required int Id { get; init; }
    public GadgetTriggerType TriggerType { get; }

    protected GadgetTrigger(GadgetTriggerType triggerType)
    {
        TriggerType = triggerType;
    }

    protected static void RegisterCauseAndEffectData(int gadgetBehaviourId)
    {
        LevelScreen.CauseAndEffectManager.RegisterCauseAndEffectData(new CauseAndEffectData(gadgetBehaviourId));
    }

    protected static void RegisterCauseAndEffectData(int gadgetBehaviourId, int lemmingId)
    {
        LevelScreen.CauseAndEffectManager.RegisterCauseAndEffectData(new CauseAndEffectData(gadgetBehaviourId, lemmingId));
    }

    public abstract void DetectTrigger();

    public bool Equals(GadgetTrigger? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => TriggerName.ToString();

    public static bool operator ==(GadgetTrigger left, GadgetTrigger right) => left.Id == right.Id;
    public static bool operator !=(GadgetTrigger left, GadgetTrigger right) => left.Id != right.Id;
}
