using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public sealed class OrGateGadget : LogicGateGadget, IPerfectHasher<OrGateGadget.OrGateGadgetInput>
{
    private readonly int _numberOfInputs;
    private readonly SimpleSet<OrGateGadget, OrGateGadgetInput> _set;

    public OrGateGadget(
        int id,
        Orientation orientation,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation)
    {
        if (inputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        _numberOfInputs = inputNames.Length;
        _set = new SimpleSet<OrGateGadget, OrGateGadgetInput>(this, false);

        for (var i = 0; i < inputNames.Length; i++)
        {
            var input = new OrGateGadgetInput(i, inputNames[i], this);
            RegisterInput(input);
        }
    }

    public override void EvaluateInputs()
    {
        Output.SetSignal(_set.Count > 0);
    }

    int IPerfectHasher<OrGateGadgetInput>.NumberOfItems => _numberOfInputs;
    int IPerfectHasher<OrGateGadgetInput>.Hash(OrGateGadgetInput item) => item.Id;
    OrGateGadgetInput IPerfectHasher<OrGateGadgetInput>.UnHash(int index) => throw new NotSupportedException();

    private sealed class OrGateGadgetInput : IGadgetInput
    {
        public readonly int Id;
        private readonly OrGateGadget _gadget;
        public string InputName { get; }

        public OrGateGadgetInput(
            int id,
            string inputName,
            OrGateGadget gadget)
        {
            Id = id;
            InputName = inputName;
            _gadget = gadget;
        }

        public void OnRegistered()
        {
        }

        public void ReactToSignal(bool signal)
        {
            if (signal)
            {
                _gadget._set.Add(this);
            }
            else
            {
                _gadget._set.Remove(this);
            }
            _gadget.EvaluateInputs();
        }
    }
}