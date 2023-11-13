using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class InputController : IPerfectHasher<Keys>
{
    private const int NumberOfKeys = 256;

    private readonly List<(Keys, KeyAction)> _keyMapping = new();
    private readonly SimpleSet<Keys> _keys;
    private readonly List<KeyAction> _keyActions = new();

    private int _previousScrollValue;

    public int MouseX { get; private set; }
    public int MouseY { get; private set; }
    public int ScrollDelta { get; private set; }

    public MouseButtonAction LeftMouseButtonAction { get; } = new(0, "Left Mouse Button");
    public MouseButtonAction RightMouseButtonAction { get; } = new(1, "Right Mouse Button");
    public MouseButtonAction MiddleMouseButtonAction { get; } = new(2, "Middle Mouse Button");
    public MouseButtonAction MouseButton4Action { get; } = new(3, "Mouse Button 4");
    public MouseButtonAction MouseButton5Action { get; } = new(4, "Mouse Button 5");

    public InputController()
    {
        _keys = new SimpleSet<Keys>(this);
    }

    public KeyAction CreateKeyAction(string actionName)
    {
        var keyAction = new KeyAction(_keyActions.Count, actionName);
        _keyActions.Add(keyAction);
        return keyAction;
    }

    public void Bind(Keys keyCode, KeyAction keyAction)
    {
        _keyMapping.Add((keyCode, keyAction));
    }

    public void ValidateKeyActions()
    {
        _keyActions.ValidateUniqueIds();
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
        var currentlyPressedKeys = Keyboard.GetState().GetPressedKeys();
        _keys.Clear();
        for (var i = 0; i < currentlyPressedKeys.Length; i++)
        {
            _keys.Add(currentlyPressedKeys[i]);
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

        LeftMouseButtonAction.UpdateState();
        RightMouseButtonAction.UpdateState();
        MiddleMouseButtonAction.UpdateState();

        MouseButton4Action.UpdateState();
        MouseButton5Action.UpdateState();

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