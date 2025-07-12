using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public interface IFunctionalGadget
{
    bool TryGetInputWithName(GadgetInputName inputName, [MaybeNullWhen(false)] out GadgetLinkInput gadgetInput);
}
