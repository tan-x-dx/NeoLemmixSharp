using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class InputController : IPerfectHasher<Keys>
{
    private const int NumberOfKeys = 256;

    private readonly List<(Keys, InputAction)> _keyMapping = new();
    private readonly SimpleSet<Keys> _keys;
    private readonly List<InputAction> _inputActions = new();

    private int _previousScrollValue;

    public int MouseX { get; private set; }
    public int MouseY { get; private set; }
    public int ScrollDelta { get; private set; }

    public InputAction LeftMouseButtonAction { get; }
    public InputAction RightMouseButtonAction { get; }
    public InputAction MiddleMouseButtonAction { get; }
    public InputAction MouseButton4Action { get; }
    public InputAction MouseButton5Action { get; }

    public InputController()
    {
        _keys = new SimpleSet<Keys>(this, false);

        LeftMouseButtonAction = CreateInputAction("Left Mouse Button");
        RightMouseButtonAction = CreateInputAction("Right Mouse Button");
        MiddleMouseButtonAction = CreateInputAction("Middle Mouse Button");
        MouseButton4Action = CreateInputAction("Mouse Button 4");
        MouseButton5Action = CreateInputAction("Mouse Button 5");
    }

    public InputAction CreateInputAction(string actionName)
    {
        var inputAction = new InputAction(_inputActions.Count, actionName);
        _inputActions.Add(inputAction);
        return inputAction;
    }

    public void Bind(Keys keyCode, InputAction inputAction)
    {
        _keyMapping.Add((keyCode, inputAction));
    }

    public void ValidateInputActions()
    {
        IdEquatableItemHelperMethods.ValidateUniqueIds<InputAction>(CollectionsMarshal.AsSpan(_inputActions));
        _inputActions.Sort(IdEquatableItemHelperMethods.Compare);
    }

    public void Tick()
    {
        var inputActionsSpan = CollectionsMarshal.AsSpan(_inputActions);

        foreach (var keyAction in inputActionsSpan)
        {
            keyAction.UpdateState();
        }

        var keyMappingSpan = CollectionsMarshal.AsSpan(_keyMapping);

        foreach (var (keyValue, action) in keyMappingSpan)
        {
            if (_keys.Contains(keyValue))
            {
                inputActionsSpan[action.Id].DoPress();
            }
        }

        UpdateKeyStates();
        UpdateMouseButtonStates();
    }

    public void ClearAllInputActions()
    {
        _keys.Clear();

        var inputActionsSpan = CollectionsMarshal.AsSpan(_inputActions);

        foreach (var inputAction in inputActionsSpan)
        {
            inputAction.Clear();
        }
    }

    private void UpdateKeyStates()
    {
        var currentlyPressedKeys = Keyboard.GetState().GetPressedKeys().AsSpan();
        _keys.Clear();
        foreach (var key in currentlyPressedKeys)
        {
            _keys.Add(key);
        }
    }

    private void UpdateMouseButtonStates()
    {
        var mouseState = Mouse.GetState();
        MouseX = mouseState.X;
        MouseY = mouseState.Y;

        var currentScrollValue = mouseState.ScrollWheelValue;
        ScrollDelta = Math.Sign(currentScrollValue - _previousScrollValue);
        _previousScrollValue = currentScrollValue;

        LeftMouseButtonAction.ActionState |= (ulong)mouseState.LeftButton;
        RightMouseButtonAction.ActionState |= (ulong)mouseState.RightButton;
        MiddleMouseButtonAction.ActionState |= (ulong)mouseState.MiddleButton;

        MouseButton4Action.ActionState |= (ulong)mouseState.XButton1;
        MouseButton5Action.ActionState |= (ulong)mouseState.XButton2;
    }

    int IPerfectHasher<Keys>.NumberOfItems => NumberOfKeys;
    int IPerfectHasher<Keys>.Hash(Keys item) => (int)item;
    Keys IPerfectHasher<Keys>.UnHash(int index) => (Keys)index;
}