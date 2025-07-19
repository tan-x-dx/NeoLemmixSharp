using NeoLemmixSharp.Engine.Level.Gadgets.Triggers;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public interface IFunctionalGadget
{
    bool TryGetInputWithName(GadgetTriggerName inputName, [MaybeNullWhen(false)] out GadgetLinkTrigger gadgetInput);
}
