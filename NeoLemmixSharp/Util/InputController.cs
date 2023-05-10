using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.Util;

public abstract class InputController<T>
    where T : IKeyAction
{
    private readonly Dictionary<int, T> _keyMapping;
    private readonly IBitArray _keys;

    private IBitArray _previousKeyActions;
    private IBitArray _currentKeyActions;

    private int _currentScrollValue;
    private int _previousScrollValue;

    private bool _currentMouseLeftDown;
    private bool _previousMouseLeftDown;

    private bool _currentMouseRightDown;
    private bool _previousMouseRightDown;

    public int MouseX { get; private set; }
    public int MouseY { get; private set; }

    public ScrollDelta ScrollDelta { get; private set; }
    public MouseButtonStatus LeftMouseButtonStatus { get; private set; }
    public MouseButtonStatus RightMouseButtonStatus { get; private set; }

    protected InputController()
    {
        _keyMapping = new Dictionary<int, T>();
        _keys = new ArrayBasedBitArray(256);
        _currentKeyActions = new IntBasedBitArray();
        _previousKeyActions = new IntBasedBitArray();
    }

    public void Update()
    {
        _currentKeyActions.Clear();

        foreach (var (keyValue, action) in _keyMapping)
        {
            if (_keys.GetBit(keyValue))
            {
                _currentKeyActions.SetBit(action.Id);
            }
        }

        (_previousKeyActions, _currentKeyActions) = (_currentKeyActions, _previousKeyActions);

        UpdateKeysDown();
        UpdateMouseState();
    }

    protected void Bind(Keys keyCode, T keyAction)
    {
        _keyMapping.Add((int)keyCode, keyAction);
    }

    public KeyStatus CheckKeyDown(T keyAction)
    {
        var previouslyDown = _previousKeyActions.GetBit(keyAction.Id)
            ? KeyStatus.KeyPressed
            : KeyStatus.KeyUnpressed;
        var currentlyDown = _currentKeyActions.GetBit(keyAction.Id)
            ? KeyStatus.KeyReleased
            : KeyStatus.KeyUnpressed;

        return previouslyDown | currentlyDown;
    }

    public void ReleaseAllKeys()
    {
        _keys.Clear();
    }

    private void UpdateKeysDown()
    {
        var keyboardState = Keyboard.GetState();
        var currentlyPressedKeys = keyboardState.GetPressedKeys();
        _keys.Clear();
        for (var i = 0; i < currentlyPressedKeys.Length; i++)
        {
            _keys.SetBit((int)currentlyPressedKeys[i]);
        }
    }

    private void UpdateMouseState()
    {
        var mouseState = Mouse.GetState();
        MouseX = mouseState.X;
        MouseY = mouseState.Y;

        _previousScrollValue = _currentScrollValue;
        _currentScrollValue = mouseState.ScrollWheelValue;

        if (_previousScrollValue < _currentScrollValue)
        {
            ScrollDelta = ScrollDelta.Positive;
        }
        else if (_previousScrollValue > _currentScrollValue)
        {
            ScrollDelta = ScrollDelta.Negative;
        }
        else
        {
            ScrollDelta = ScrollDelta.None;
        }

        _previousMouseLeftDown = _currentMouseLeftDown;
        _currentMouseLeftDown = mouseState.LeftButton == ButtonState.Pressed;
        var leftMousePreviouslyDown = _previousMouseLeftDown
            ? MouseButtonStatus.MouseButtonPressed
            : MouseButtonStatus.MouseButtonUnpressed;
        var leftMouseCurrentlyDown = _currentMouseLeftDown
            ? MouseButtonStatus.MouseButtonReleased
            : MouseButtonStatus.MouseButtonUnpressed;
        LeftMouseButtonStatus = leftMouseCurrentlyDown | leftMousePreviouslyDown;

        _previousMouseRightDown = _currentMouseRightDown;
        _currentMouseRightDown = mouseState.RightButton == ButtonState.Pressed;
        var rightMousePreviouslyDown = _previousMouseRightDown
            ? MouseButtonStatus.MouseButtonPressed
            : MouseButtonStatus.MouseButtonUnpressed;
        var rightMouseCurrentlyDown = _currentMouseRightDown
            ? MouseButtonStatus.MouseButtonReleased
            : MouseButtonStatus.MouseButtonUnpressed;
        RightMouseButtonStatus = rightMouseCurrentlyDown | rightMousePreviouslyDown;
    }
}

[Flags]
public enum KeyStatus
{
    KeyUnpressed = 0,
    KeyPressed = 1,
    KeyReleased = 2,
    KeyHeld = 3
}

[Flags]
public enum MouseButtonStatus
{
    MouseButtonUnpressed = 0,
    MouseButtonPressed = 1,
    MouseButtonReleased = 2,
    MouseButtonHeld = 3
}

public enum ScrollDelta
{
    Negative = -1,
    None = 0,
    Positive = 1
}