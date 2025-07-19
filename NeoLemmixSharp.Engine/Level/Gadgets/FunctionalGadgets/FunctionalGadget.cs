using NeoLemmixSharp.Engine.Level.Gadgets.Triggers;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class FunctionalGadget<TInput> : GadgetBase, IFunctionalGadget
    where TInput : GadgetLinkTrigger
{
    private readonly TInput[] _inputs;
    private int _gadgetIndex;

    protected FunctionalGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        int expectedNumberOfInputs)
        : base(gadgetName)
    {
        _inputs = new TInput[expectedNumberOfInputs];
    }

    protected void RegisterInput(TInput gadgetInput)
    {
        _inputs[_gadgetIndex++] = gadgetInput;
    }

    public bool TryGetInputWithName(GadgetTriggerName inputName, [MaybeNullWhen(false)] out GadgetLinkTrigger gadgetInput)
    {
        for (var i = 0; i < _gadgetIndex; i++)
        {
            var input = _inputs[i];
            if (input.TriggerName.Equals(inputName))
            {
                gadgetInput = input;
                return true;
            }
        }

        gadgetInput = null;
        return false;
    }
}
