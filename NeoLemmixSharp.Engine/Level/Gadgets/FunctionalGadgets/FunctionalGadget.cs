using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets;

public abstract class FunctionalGadget : GadgetBase
{
    private readonly SimpleList<GadgetInput> _inputs;

    protected FunctionalGadget(int expectedNumberOfInputs)
    {
        _inputs = new SimpleList<GadgetInput>(expectedNumberOfInputs);
    }

    protected void RegisterInput(GadgetInput gadgetInput)
    {
        _inputs.Add(gadgetInput);
    }

    public bool TryGetInputWithName(string inputName, [MaybeNullWhen(false)] out GadgetInput gadgetInput)
    {
        var span = _inputs.AsReadOnlySpan();
        for (var i = 0; i < _inputs.Count; i++)
        {
            var input = span[i];
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
