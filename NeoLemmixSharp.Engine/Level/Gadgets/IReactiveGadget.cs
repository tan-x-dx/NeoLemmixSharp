using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IReactiveGadget
{
    IGadgetInput? GetInputWithName(string inputName);
}