using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

public sealed class GadgetOutput
{
    private readonly List<IGadgetInput> _inputs = new();
    private bool _currentSignal;

    public void RegisterInput(IGadgetInput input)
    {
        _inputs.Add(input);
        input.OnRegistered();
    }

    public void SetSignal(bool newSignal)
    {
        if (_currentSignal == newSignal)
            return;

        _currentSignal = newSignal;

        var span = CollectionsMarshal.AsSpan(_inputs);
        foreach (var gadgetInput in span)
        {
            gadgetInput.ReactToSignal(_currentSignal);
        }
    }
}