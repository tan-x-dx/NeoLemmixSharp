using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Util;

public abstract class InputController<T>
    where T : class, IKeyAction
{
    private readonly Dictionary<int, T> _keyMapping;
    private readonly bool[] _keys;
    private readonly int[] _keyActionStatuses;

    private int _previousScrollValue;

    public int MouseX { get; private set; }
    public int MouseY { get; private set; }

    public int ScrollDelta { get; private set; }
    public int LeftMouseButtonStatus { get; private set; }
    public int RightMouseButtonStatus { get; private set; }

    protected InputController(int numberOfKeyboardInputs)
    {
        _keyMapping = new Dictionary<int, T>();
        _keys = new bool[256];

        _keyActionStatuses = new int[numberOfKeyboardInputs];
    }

    public void Update()
    {
        for (var i = 0; i < _keyActionStatuses.Length; i++)
        {
            _keyActionStatuses[i] = (_keyActionStatuses[i] << 1) & 2;
        }

        foreach (var (keyValue, action) in _keyMapping)
        {
            if (_keys[keyValue])
            {
                _keyActionStatuses[action.Id] |= KeyStatusConsts.KeyPressed;
            }
        }

        UpdateKeysDown();
        UpdateMouseState();
    }

    protected void Bind(Keys keyCode, T keyAction)
    {
        _keyMapping.Add((int)keyCode, keyAction);
    }

    public int CheckKeyDown(T keyAction)
    {
        return _keyActionStatuses[keyAction.Id];
    }

    public void ReleaseAllKeys()
    {
        Array.Clear(_keys);
    }

    private void UpdateKeysDown()
    {
        var currentlyPressedKeys = Keyboard.GetState().GetPressedKeys();
        Array.Clear(_keys);
        for (var i = 0; i < currentlyPressedKeys.Length; i++)
        {
            _keys[(int)currentlyPressedKeys[i]] = true;
        }
    }

    private void UpdateMouseState()
    {
        var mouseState = Mouse.GetState();
        MouseX = mouseState.X;
        MouseY = mouseState.Y;

        var currentScrollValue = mouseState.ScrollWheelValue;
        ScrollDelta = Math.Sign(currentScrollValue - _previousScrollValue);
        _previousScrollValue = currentScrollValue;

        LeftMouseButtonStatus = ((LeftMouseButtonStatus << 1) & 2) | (int)mouseState.LeftButton;
        RightMouseButtonStatus = ((RightMouseButtonStatus << 1) & 2) | (int)mouseState.RightButton;
    }
}

public static class KeyStatusConsts
{
    public const int KeyUnpressed = 0;
    public const int KeyPressed = 1;
    public const int KeyReleased = 2;
    public const int KeyHeld = 3;
}

public static class MouseButtonStatusConsts
{
    public const int MouseButtonUnpressed = 0;
    public const int MouseButtonPressed = 1;
    public const int MouseButtonReleased = 2;
    public const int MouseButtonHeld = 3;
}

public static class ScrollDeltaConsts
{
    public const int Negative = -1;
    public const int None = 0;
    public const int Positive = 1;
}