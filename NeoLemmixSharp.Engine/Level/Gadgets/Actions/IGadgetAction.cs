using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Actions;

public interface IGadgetAction
{
    void PerformAction(Lemming lemming);
}