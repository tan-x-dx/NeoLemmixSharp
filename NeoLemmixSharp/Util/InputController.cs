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
    public MouseButtonAction LeftMouseButtonAction { get; }
    public MouseButtonAction RightMouseButtonAction { get; }
    public MouseButtonAction MiddleMouseButtonAction { get; }
    public MouseButtonAction MouseButton4Action { get; }
    public MouseButtonAction MouseButton5Action { get; }

    protected InputController(int numberOfKeyboardInputs)
    {
        _keyMapping = new List<(int, KeyAction)>();
        _keys = new bool[256];

        _keyActions = new KeyAction[numberOfKeyboardInputs];

        LeftMouseButtonAction = new MouseButtonAction(0, "Left Mouse Button");
        RightMouseButtonAction = new MouseButtonAction(1, "Right Mouse Button");
        MiddleMouseButtonAction = new MouseButtonAction(2, "Middle Mouse Button");
        MouseButton4Action = new MouseButtonAction(3, "Mouse Button 4");
        MouseButton5Action = new MouseButtonAction(4, "Button 5");
    }

    public void Update()
    {
        for (var i = 0; i < _keyActions.Length; i++)
        {
            _keyActions[i].UpdateStatus();
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

        LeftMouseButtonAction.UpdateState(mouseState.LeftButton);
        RightMouseButtonAction.UpdateState(mouseState.RightButton);
        MiddleMouseButtonAction.UpdateState(mouseState.MiddleButton);

        MouseButton4Action.UpdateState(mouseState.XButton1);
        MouseButton5Action.UpdateState(mouseState.XButton2);
    }
}