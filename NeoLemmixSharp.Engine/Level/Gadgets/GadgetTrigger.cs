using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetTrigger : IIdEquatable<GadgetTrigger>
{
    private const int IndeterminateTriggerValue = -1;
    private const int DefinitelyNotTriggeredValue = 0;
    private const int DefinitelyTriggeredValue = 1;

    public required GadgetTriggerName TriggerName { get; init; }
    public required int Id { get; init; }
    public GadgetTriggerType TriggerType { get; }
    public TriggerEvaluation Evaluation { get; private set; }

    protected GadgetTrigger(GadgetTriggerType triggerType)
    {
        TriggerType = triggerType;
    }

    public void Reset()
    {
        Evaluation = TriggerEvaluation.Indeterminate;
    }

    public void DetermineTrigger(bool isTriggered)
    {
        var triggerNum = isTriggered ? DefinitelyTriggeredValue : DefinitelyNotTriggeredValue;
        Evaluation = (TriggerEvaluation)triggerNum;
    }

    protected static void RegisterCauseAndEffectData(int gadgetBehaviourId)
    {
        LevelScreen.CauseAndEffectManager.RegisterCauseAndEffectData(new CauseAndEffectData(gadgetBehaviourId));
    }

    protected static void RegisterCauseAndEffectData(int gadgetBehaviourId, int lemmingId)
    {
        LevelScreen.CauseAndEffectManager.RegisterCauseAndEffectData(new CauseAndEffectData(gadgetBehaviourId, lemmingId));
    }

    public abstract void DetectTrigger(GadgetBase parentGadget);

    public bool Equals(GadgetTrigger? other) => other is not null && Id == other.Id;
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => TriggerName.ToString();

    public static bool operator ==(GadgetTrigger left, GadgetTrigger right) => left.Id == right.Id;
    public static bool operator !=(GadgetTrigger left, GadgetTrigger right) => left.Id != right.Id;

    public enum TriggerEvaluation
    {
        Indeterminate = IndeterminateTriggerValue,
        DefinitelyNotTriggered = DefinitelyNotTriggeredValue,
        DefinitelyTriggered = DefinitelyTriggeredValue,
    }
}
