using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetActions;

public interface IGadgetBehaviour
{
    void PerformAction(Lemming lemming);
}