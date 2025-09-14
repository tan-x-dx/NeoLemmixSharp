using NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public interface IGadgetLinkTrigger
{
    OutputSignalBehaviour? InputSignalBehaviour { get; set; }
    void ReactToSignal();
}
