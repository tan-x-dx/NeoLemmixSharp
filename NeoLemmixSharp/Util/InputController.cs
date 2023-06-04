using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Util;

public abstract class InputController
{
    private readonly List<(int, KeyAction)> _keyMapping;
    private readonly bool[] _keys;
    private readonly KeyAction[] _keyActions;

    private int _previousScrollValue;

    public int MouseX { get; private set; }
    public int MouseY { get; private set; }

    public int ScrollDelta { get; private set; }
    public int LeftMouseButtonStatus { get; private set; }
    public int RightMouseButtonStatus { get; private set; }

    protected InputController(int numberOfKeyboardInputs)
    {
        _keyMapping = new List<(int, KeyAction)>();
        _keys = new bool[256];

        _keyActions = new KeyAction[numberOfKeyboardInputs];
    }

    public void Update()
    {
        for (var i = 0; i < _keyActions.Length; i++)
        {
            _keyActions[i].KeyState = (_keyActions[i].KeyState << 1) & 2;
        }

        for (var index = 0; index < _keyMapping.Count; index++)
        {
            var (keyValue, action) = _keyMapping[index];
            if (_keys[keyValue])
            {
                _keyActions[action.Id].KeyState |= KeyStatusConsts.KeyPressed;
            }
        }

        UpdateKeysDown();
        UpdateMouseState();
    }

    protected void Bind(Keys keyCode, KeyAction keyAction)
    {
        _keyMapping.Add(((int)keyCode, keyAction));
        _keyActions[keyAction.Id] = keyAction;
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