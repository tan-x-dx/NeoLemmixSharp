using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetBehaviours;

public interface IGadgetBehaviour
{
    void PerformAction(Lemming lemming);
}