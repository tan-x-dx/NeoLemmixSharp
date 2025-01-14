using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.FunctionalGadgets.BinaryLogic;

public sealed class AndGateGadget : LogicGateGadget, IPerfectHasher<AndGateGadget.AndGateGadgetInput>
{
    private readonly int _numberOfInputs;
    private readonly SimpleSet<AndGateGadget, AndGateGadgetInput> _set;

    public AndGateGadget(
        int id,
        Orientation orientation,
        ReadOnlySpan<string> inputNames)
        : base(id, orientation)
    {
        if (inputNames.Length < 2)
            throw new ArgumentException("Expected at least 2 inputs!");

        _numberOfInputs = inputNames.Length;
        _set = new SimpleSet<AndGateGadget, AndGateGadgetInput>(this, false);

        for (var i = 0; i < inputNames.Length; i++)
        {
            var input = new AndGateGadgetInput(i, inputNames[i], this);
            RegisterInput(input);
        }
    }

    public override void EvaluateInputs()
    {
        Output.SetSignal(_set.Count == _numberOfInputs);
    }

    int IPerfectHasher<AndGateGadgetInput>.NumberOfItems => _numberOfInputs;
    int IPerfectHasher<AndGateGadgetInput>.Hash(AndGateGadgetInput item) => item.Id;
    AndGateGadgetInput IPerfectHasher<AndGateGadgetInput>.UnHash(int index) => throw new NotSupportedException();

    private sealed class AndGateGadgetInput : IGadgetInput
    {
        public readonly int Id;
        private readonly AndGateGadget _gadget;
        public string InputName { get; }

        public AndGateGadgetInput(
            int id,
            string inputName,
            AndGateGadget gadget)
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
