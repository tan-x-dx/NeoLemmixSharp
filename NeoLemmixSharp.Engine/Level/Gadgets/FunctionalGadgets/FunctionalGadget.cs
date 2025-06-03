using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class FunctionalGadget<TInput> : GadgetBase
    where TInput : GadgetInput
{
    private readonly TInput[] _inputs;
    private int _gadgetIndex;

    protected FunctionalGadget(
        string gadgetName,
        GadgetState[] states,
        bool startActive,
        int expectedNumberOfInputs)
        : base(gadgetName, states, startActive ? 1 : 0)
    {
        _inputs = new TInput[expectedNumberOfInputs];
    }

    protected void RegisterInput(TInput gadgetInput)
    {
        _inputs[_gadgetIndex++] = gadgetInput;
    }

    public bool TryGetInputWithName(string inputName, [MaybeNullWhen(false)] out TInput gadgetInput)
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
