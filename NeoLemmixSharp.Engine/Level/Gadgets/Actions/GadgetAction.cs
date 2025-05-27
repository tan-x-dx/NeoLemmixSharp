using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level.Gadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public abstract class GadgetAction : IComparable<GadgetAction>
{
    public GadgetActionType ActionType { get; }

    protected GadgetAction(GadgetActionType actionType)
    {
        ActionType = actionType;
    }

    public abstract void PerformAction(Lemming lemming);

    public int CompareTo(GadgetAction? other)
    {
        if (other == null) return 1;

        return (int)ActionType - (int)other.ActionType;
    }
}