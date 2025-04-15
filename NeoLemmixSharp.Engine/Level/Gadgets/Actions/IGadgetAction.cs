using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public interface IGadgetAction
{
    GadgetActionType ActionType { get; }
    void PerformAction(Lemming lemming);
}