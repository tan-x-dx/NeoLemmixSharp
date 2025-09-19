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

    public required GadgetBehaviour[] Behaviours { private get; init; }
    protected GadgetBase ParentGadget = null!;
    protected GadgetState ParentState = null!;

    protected GadgetTrigger(GadgetTriggerType triggerType)
    {
        TriggerType = triggerType;
    }

    public void SetParentData(GadgetBase parentGadget, GadgetState parentState)
    {
        ParentGadget = parentGadget;
        ParentState = parentState;
    }

    public void Reset()
    {
        var parentGadget = ParentGadget;
        var parentState = ParentState;
        var currentParentGadgetState = parentGadget.CurrentState;
        if (currentParentGadgetState == parentState)
        {
            Evaluation = TriggerEvaluation.Indeterminate;
        }
        else
        {
            Evaluation = TriggerEvaluation.DefinitelyNotTriggered;
            MarkAsEvaluated();
        }
    }

    public void DetermineTrigger(bool isTriggered)
    {
        if (Evaluation != TriggerEvaluation.Indeterminate)
            return;

        var triggerNum = isTriggered ? DefinitelyTriggeredValue : DefinitelyNotTriggeredValue;
        Evaluation = (TriggerEvaluation)triggerNum;
    }

    protected void MarkAsEvaluated() => LevelScreen.GadgetManager.MarkTriggerAsEvaluated(this);

    protected void TriggerBehaviours()
    {
        foreach (var behaviour in Behaviours)
        {
            LevelScreen.GadgetManager.RegisterCauseAndEffectData(new CauseAndEffectData(behaviour.Id, 1));
        }
    }

    public abstract void DetectTrigger();

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
