using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;
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
    private TriggerEvaluation _evaluation;

    public required GadgetBehaviour[] Behaviours { private get; init; }
    protected GadgetBase ParentGadget = null!;
    protected GadgetState ParentState = null!;

    public bool IsIndeterminate => _evaluation == TriggerEvaluation.Indeterminate;

    protected GadgetTrigger(GadgetTriggerType triggerType)
    {
        TriggerType = triggerType;
    }

    public void SetParentData(GadgetBase parentGadget, GadgetState parentState)
    {
        Debug.Assert(ParentGadget is null);
        Debug.Assert(ParentState is null);

        ParentGadget = parentGadget;
        ParentState = parentState;

        foreach (var behaviour in Behaviours)
        {
            behaviour.SetParentGadget(parentGadget);
        }
    }

    public void Reset()
    {
        var parentGadget = ParentGadget;
        var parentState = ParentState;
        var currentParentGadgetState = parentGadget.CurrentState;
        if (currentParentGadgetState == parentState)
        {
            _evaluation = TriggerEvaluation.Indeterminate;
        }
        else
        {
            _evaluation = TriggerEvaluation.DefinitelyNotTriggered;
            MarkAsEvaluated();
        }
    }

    public void DetermineTrigger(bool isTriggered)
    {
        if (_evaluation != TriggerEvaluation.Indeterminate)
            return;

        var triggerNum = isTriggered ? DefinitelyTriggeredValue : DefinitelyNotTriggeredValue;
        _evaluation = (TriggerEvaluation)triggerNum;
    }

    protected void MarkAsEvaluated() => LevelScreen.GadgetManager.MarkTriggerAsEvaluated(this);

    protected void TriggerBehaviours()
    {
        foreach (var behaviour in Behaviours)
        {
            LevelScreen.GadgetManager.RegisterCauseAndEffectData(behaviour);
        }
    }

    public abstract void DetectTrigger();

    public bool Equals(GadgetTrigger? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }

    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => TriggerName.ToString();

    public static bool operator ==(GadgetTrigger? left, GadgetTrigger? right)
    {
        var leftValue = -1;
        if (left is not null) leftValue = left.Id;
        var rightValue = -1;
        if (right is not null) rightValue = right.Id;
        return leftValue == rightValue;
    }
    public static bool operator !=(GadgetTrigger? left, GadgetTrigger? right) => !(left == right);

    private enum TriggerEvaluation
    {
        Indeterminate = IndeterminateTriggerValue,
        DefinitelyNotTriggered = DefinitelyNotTriggeredValue,
        DefinitelyTriggered = DefinitelyTriggeredValue,
    }
}
