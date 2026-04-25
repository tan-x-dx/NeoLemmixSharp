using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetTrigger : IEquatable<GadgetTrigger>
{
    private const int IndeterminateTriggerValue = -1;
    private const int DefinitelyNotTriggeredValue = 0;
    private const int DefinitelyTriggeredValue = 1;

    public required GadgetTriggerName TriggerName { get; init; }
    public required int Id { get; init; }
    public GadgetTriggerType TriggerType { get; }
    private int _evaluation;

    public required GadgetBehaviour[] Behaviours { private get; init; }
    protected GadgetBase ParentGadget = null!;
    protected GadgetState ParentState = null!;

    public bool IsIndeterminate => _evaluation == IndeterminateTriggerValue;

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
            _evaluation = IndeterminateTriggerValue;
        }
        else
        {
            _evaluation = IndeterminateTriggerValue;
            MarkAsEvaluated();
        }
    }

    public void DetermineTrigger(bool isTriggered)
    {
        if (_evaluation != IndeterminateTriggerValue)
            return;

        var triggerNum = isTriggered ? DefinitelyTriggeredValue : DefinitelyNotTriggeredValue;
        _evaluation = triggerNum;
    }

    protected void MarkAsEvaluated() => LevelScreen.GadgetManager.MarkTriggerAsEvaluated(this);

    protected void TriggerBehaviours()
    {
        foreach (var behaviour in Behaviours)
        {
            behaviour.PerformBehaviour();
        }
    }

    public abstract void DetectTrigger();

    public bool Equals(GadgetTrigger? other)
    {
        var otherId = -1;
        if (other is not null) otherId = other.Id;
        return Id == otherId;
    }

    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetTrigger other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => TriggerName.ToString();

    public static bool operator ==(GadgetTrigger? left, GadgetTrigger? right)
    {
        var leftId = -1;
        if (left is not null) leftId = left.Id;
        var rightId = -1;
        if (right is not null) rightId = right.Id;
        return leftId == rightId;
    }
    public static bool operator !=(GadgetTrigger? left, GadgetTrigger? right) => !(left == right);
}
