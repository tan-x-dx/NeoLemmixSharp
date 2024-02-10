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
    private readonly List<InputAction> _keyActions = new();

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
        _keys = new SimpleSet<Keys>(this);

        LeftMouseButtonAction = CreateKeyAction("Left Mouse Button");
        RightMouseButtonAction = CreateKeyAction("Right Mouse Button");
        MiddleMouseButtonAction = CreateKeyAction("Middle Mouse Button");
        MouseButton4Action = CreateKeyAction("Mouse Button 4");
        MouseButton5Action = CreateKeyAction("Mouse Button 5");
    }

    public InputAction CreateKeyAction(string actionName)
    {
        var keyAction = new InputAction(_keyActions.Count, actionName);
        _keyActions.Add(keyAction);
        return keyAction;
    }

    public void Bind(Keys keyCode, InputAction keyAction)
    {
        _keyMapping.Add((keyCode, keyAction));
    }

    public void ValidateKeyActions()
    {
        IdEquatableItemHelperMethods.ValidateUniqueIds<InputAction>(CollectionsMarshal.AsSpan(_keyActions));
        _keyActions.Sort(IdEquatableItemHelperMethods.Compare);
    }

    public void Tick()
    {
        var keyActionsSpan = CollectionsMarshal.AsSpan(_keyActions);

        foreach (var keyAction in keyActionsSpan)
        {
            keyAction.UpdateState();
        }

        var keyMappingSpan = CollectionsMarshal.AsSpan(_keyMapping);

        foreach (var (keyValue, action) in keyMappingSpan)
        {
            if (_keys.Contains(keyValue))
            {
                keyActionsSpan[action.Id].ActionState |= InputAction.ActionPressed;
            }
        }

        UpdateKeyStates();
        UpdateMouseButtonStates();
    }

    public void ClearAllKeys()
    {
        _keys.Clear();

        var keyActionsSpan = CollectionsMarshal.AsSpan(_keyActions);

        foreach (var keyAction in keyActionsSpan)
        {
            keyAction.Clear();
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