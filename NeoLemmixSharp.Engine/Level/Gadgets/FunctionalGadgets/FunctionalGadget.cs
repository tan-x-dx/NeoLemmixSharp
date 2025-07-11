using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class FunctionalGadget<TInput> : GadgetBase, IFunctionalGadget
    where TInput : GadgetInput
{
    private readonly TInput[] _inputs;
    private int _gadgetIndex;

    protected FunctionalGadget(
        string gadgetName,
        GadgetState[] states,
        int initialStateIndex,
        int expectedNumberOfInputs)
        : base(gadgetName, states, initialStateIndex)
    {
        _inputs = new TInput[expectedNumberOfInputs];
    }

    protected void RegisterInput(TInput gadgetInput)
    {
        _inputs[_gadgetIndex++] = gadgetInput;
    }

    public bool TryGetInputWithName(GadgetInputName inputName, [MaybeNullWhen(false)] out GadgetInput gadgetInput)
    {
        for (var i = 0; i < _gadgetIndex; i++)
        {
            var input = _inputs[i];
            if (string.Equals(input.InputName, inputName))
            {
                gadgetInput = input;
                return true;
            }
        }

        gadgetInput = null;
        return false;
    }
}
