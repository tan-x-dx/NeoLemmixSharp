using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class InputController :
    IPerfectHasher<InputAction>,
    IBitBufferCreator<InputController.KeysBitBuffer, Keys>
{
    private const int NumberOfKeys = 256;

    private readonly List<KeyToInputMapping> _keyMapping = new(16);
    private readonly BitArraySet<InputController, KeysBitBuffer, Keys> _previouslyPressedKeys;
    private readonly BitArraySet<InputController, KeysBitBuffer, Keys> _currentlyPressedKeys;
    private readonly BitArraySet<InputController, KeysBitBuffer, Keys> _heldKeys;
    private readonly BitArraySet<InputController, KeysBitBuffer, Keys> _releasedKeys;
    private readonly List<InputAction> _inputActions = new(16);

    private int _previousScrollValue;

    public Point MousePosition { get; private set; }
    public int ScrollDelta { get; private set; }

    public InputAction LeftMouseButtonAction { get; }
    public InputAction RightMouseButtonAction { get; }
    public InputAction MiddleMouseButtonAction { get; }
    public InputAction MouseButton4Action { get; }
    public InputAction MouseButton5Action { get; }

    private KeyboardInputType _keyboardInputType;
    private KeyboardInputModifiers _inputModifiers;
    private int _numberOfFramesThisKeyHasBeenPressed;
    private char _keyboardChar;
    public bool _capsLock;

    public KeyboardInput LatestKeyboardInput() => new(_keyboardInputType, _inputModifiers, _numberOfFramesThisKeyHasBeenPressed, _keyboardChar, _capsLock);

    public BitArrayEnumerable<InputController, Keys> CurrentlyPressedKeys => _currentlyPressedKeys.AsEnumerable();
    public BitArrayEnumerable<InputController, Keys> CurrentlyHeldKeys => _heldKeys.AsEnumerable();
    public BitArrayEnumerable<InputController, Keys> JustReleasedKeys => _releasedKeys.AsEnumerable();

    public InputController()
    {
        _previouslyPressedKeys = new BitArraySet<InputController, KeysBitBuffer, Keys>(this);
        _currentlyPressedKeys = new BitArraySet<InputController, KeysBitBuffer, Keys>(this);
        _heldKeys = new BitArraySet<InputController, KeysBitBuffer, Keys>(this);
        _releasedKeys = new BitArraySet<InputController, KeysBitBuffer, Keys>(this);

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
        _keyMapping.Add(new KeyToInputMapping(keyCode, inputAction));
    }

    public void ValidateInputActions()
    {
        var inputActionsSpan = CollectionsMarshal.AsSpan(_inputActions);
        this.AssertUniqueIds(inputActionsSpan);
        inputActionsSpan.Sort(this);
    }

    public void Tick()
    {
        foreach (var keyAction in _inputActions)
        {
            keyAction.UpdateState();
        }

        UpdateKeyStates();
        UpdateMouseButtonStates();
        EvaluateCurrentKeyboardInput();

        foreach (var keyToInputMapping in _keyMapping)
        {
            if (_currentlyPressedKeys.Contains(keyToInputMapping.Key))
            {
                keyToInputMapping.InputAction.DoPress();
            }
        }
    }

    public void ClearAllInputActions()
    {
        _previouslyPressedKeys.Clear();
        _currentlyPressedKeys.Clear();
        _heldKeys.Clear();
        _releasedKeys.Clear();

        foreach (var inputAction in _inputActions)
        {
            inputAction.Clear();
        }
    }

    private unsafe void UpdateKeyStates()
    {
        // Doing it this way prevents allocation of an array.
        // The actual data we want happens to be bit-encoded
        // in the same way as a BitArraySet would do.
        // Just copy it in...
        var keyBoardState = Keyboard.GetState();
        _capsLock = keyBoardState.CapsLock;

        var keysSpan = Helpers.CreateReadOnlySpan<uint>(&keyBoardState, KeysBitBuffer.KeysBitBufferLength);

        _previouslyPressedKeys.SetFrom(_currentlyPressedKeys);
        _currentlyPressedKeys.ReadFrom(keysSpan);

        _heldKeys.SetFrom(_currentlyPressedKeys);
        _heldKeys.IntersectWith(_previouslyPressedKeys);

        _releasedKeys.SetFrom(_previouslyPressedKeys);
        _releasedKeys.ExceptWith(_currentlyPressedKeys);
    }

    private void UpdateMouseButtonStates()
    {
        var mouseState = Mouse.GetState();
        MousePosition = new Point(mouseState.X, mouseState.Y);

        var currentScrollValue = mouseState.ScrollWheelValue;
        ScrollDelta = Math.Sign(currentScrollValue - _previousScrollValue);
        _previousScrollValue = currentScrollValue;

        LeftMouseButtonAction.ActionState |= (ulong)mouseState.LeftButton;
        RightMouseButtonAction.ActionState |= (ulong)mouseState.RightButton;
        MiddleMouseButtonAction.ActionState |= (ulong)mouseState.MiddleButton;

        MouseButton4Action.ActionState |= (ulong)mouseState.XButton1;
        MouseButton5Action.ActionState |= (ulong)mouseState.XButton2;
    }

    private void EvaluateCurrentKeyboardInput()
    {
        if (_currentlyPressedKeys.Count == 0)
            goto BlankOutCharData;

        var uniqueKeyboardInputType = KeyboardInputType.None;
        var inputModifiers = KeyboardInputModifiers.None;
        var uniqueChar = (char)0;

        foreach (var key in _currentlyPressedKeys)
        {
            if (key == Keys.LeftShift || key == Keys.RightShift)
            {
                inputModifiers |= KeyboardInputModifiers.Shift;
                continue;
            }
            if (key == Keys.LeftControl || key == Keys.RightControl)
            {
                inputModifiers |= KeyboardInputModifiers.Ctrl;
                continue;
            }
            if (key == Keys.LeftAlt || key == Keys.RightAlt)
            {
                inputModifiers |= KeyboardInputModifiers.Alt;
                continue;
            }

            var currentKeyboardInputType = KeyboardHelpers.GetKeyboardInputType(key);
            if (uniqueKeyboardInputType != KeyboardInputType.None)
            {
                if (_previouslyPressedKeys.Contains(key))
                    continue;

                goto BlankOutCharData;
            }

            uniqueKeyboardInputType = currentKeyboardInputType;
            if (currentKeyboardInputType != KeyboardInputType.Character)
                continue;

            uniqueChar = KeyboardHelpers.GetCharFromKey(key);
        }

        if (_keyboardInputType == uniqueKeyboardInputType &&
            _inputModifiers == inputModifiers &&
            _keyboardChar == uniqueChar)
        {
            _numberOfFramesThisKeyHasBeenPressed++;
            return;
        }

        _keyboardInputType = uniqueKeyboardInputType;
        _inputModifiers = inputModifiers;
        _numberOfFramesThisKeyHasBeenPressed = 1;
        _keyboardChar = uniqueChar;

        return;

    BlankOutCharData:
        _keyboardInputType = KeyboardInputType.None;
        _inputModifiers = KeyboardInputModifiers.None;
        _numberOfFramesThisKeyHasBeenPressed = 0;
        _keyboardChar = (char)0;
    }

    /// <summary>
    /// Is the key currently pressed down?
    /// </summary>
    public bool IsKeyDown(Keys key) => _currentlyPressedKeys.Contains(key);
    /// <summary>
    /// Is the key currently released?
    /// </summary>
    public bool IsKeyUp(Keys key) => !_currentlyPressedKeys.Contains(key);
    /// <summary>
    /// Is the key currently pressed down, but it was previously released?
    /// </summary>
    public bool IsKeyPressed(Keys key) => _currentlyPressedKeys.Contains(key) &&
                                          !_previouslyPressedKeys.Contains(key);
    /// <summary>
    /// Is the key currently released, but it was previously pressed down?
    /// </summary>
    public bool IsKeyReleased(Keys key) => _releasedKeys.Contains(key);
    /// <summary>
    /// Is the key currently being pressed down, and it was previously pressed down?
    /// </summary>
    public bool IsKeyHeld(Keys key) => _heldKeys.Contains(key);

    int IPerfectHasher<Keys>.NumberOfItems => NumberOfKeys;
    int IPerfectHasher<Keys>.Hash(Keys item) => (int)item;
    Keys IPerfectHasher<Keys>.UnHash(int index) => (Keys)index;
    int IPerfectHasher<InputAction>.NumberOfItems => _inputActions.Count;
    int IPerfectHasher<InputAction>.Hash(InputAction item) => item.Id;
    InputAction IPerfectHasher<InputAction>.UnHash(int index) => _inputActions[index];
    void IBitBufferCreator<KeysBitBuffer, Keys>.CreateBitBuffer(out KeysBitBuffer buffer) => buffer = new();

    private readonly record struct KeyToInputMapping(Keys Key, InputAction InputAction);

    [InlineArray(KeysBitBufferLength)]
    private struct KeysBitBuffer : IBitBuffer
    {
        public const int KeysBitBufferLength = NumberOfKeys / 32;

        private uint _0;

        public readonly int Length => KeysBitBufferLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<uint> AsSpan() => MemoryMarshal.CreateSpan(ref _0, KeysBitBufferLength);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<uint> AsReadOnlySpan() => MemoryMarshal.CreateReadOnlySpan(in _0, KeysBitBufferLength);
    }
}
