using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoLemmixSharp.Util;

public abstract class KeyController<T> : ITickable
    where T : IKeyAction
{
    private readonly Dictionary<int, T> _keyMapping;
    private readonly IBitArray _keys;

    private IBitArray _previousActions;
    private IBitArray _currentActions;

    protected KeyController()
    {
        _keyMapping = new Dictionary<int, T>();
        _keys = new ArrayBasedBitArray(256);
        _currentActions = new IntBasedBitArray();
        _previousActions = new IntBasedBitArray();
    }

    public void Tick()
    {
        _currentActions.Clear();

        foreach (var (keyValue, action) in _keyMapping)
        {
            if (_keys.GetBit(keyValue))
            {
                _currentActions.SetBit(action.Id);
            }
        }

        (_previousActions, _currentActions) = (_currentActions, _previousActions);
    }

    protected void Bind(Keys keyCode, T keyAction)
    {
        _keyMapping.Add((int)keyCode, keyAction);
    }

    public KeyStatus CheckKeyDown(T keyAction)
    {
        var previouslyDown = _previousActions.GetBit(keyAction.Id)
            ? KeyStatus.KeyPressed
            : KeyStatus.KeyUnpressed;
        var currentlyDown = _currentActions.GetBit(keyAction.Id)
            ? KeyStatus.KeyReleased
            : KeyStatus.KeyUnpressed;

        return previouslyDown | currentlyDown;
    }

    public void ReleaseAllKeys()
    {
        _keys.Clear();
    }

    public void ControllerKeysDown(Keys[] currentlyPressedKeys)
    {
        _keys.Clear();
        for (var i = 0; i < currentlyPressedKeys.Length; i++)
        {
            _keys.SetBit((int)currentlyPressedKeys[i]);
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ITickable.ShouldTick => true;
}

[Flags]
public enum KeyStatus
{
    KeyUnpressed = 0,
    KeyPressed = 1,
    KeyReleased = 2,
    KeyHeld = 3
}
