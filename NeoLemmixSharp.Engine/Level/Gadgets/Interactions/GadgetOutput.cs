﻿namespace NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

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

        for (var i = 0; i < _inputs.Count; i++)
        {
            _inputs[i].ReactToSignal(_currentSignal);
        }
    }
}