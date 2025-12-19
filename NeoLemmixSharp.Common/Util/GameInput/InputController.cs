using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class InputController :
    IPerfectHasher<InputAction>,
    IBitBufferCreator<InputController.KeysBitBuffer, Keys>
{
    private const int NumberOfKeys = 256;

    private readonly SimpleList<KeyToInputMapping> _keyMapping = new(16);
    private readonly BitArraySet<InputController, KeysBitBuffer, Keys> _pressedKeys;
    private readonly BitArraySet<InputController, KeysBitBuffer, Keys> _releasedKeys;
    private readonly SimpleList<InputAction> _inputActions = new(16);

    private int _previousScrollValue;

    public Point MousePosition { get; private set; }
    public int ScrollDelta { get; private set; }

    public InputAction LeftMouseButtonAction { get; }
    public InputAction RightMouseButtonAction { get; }
    public InputAction MiddleMouseButtonAction { get; }
    public InputAction MouseButton4Action { get; }
    public InputAction MouseButton5Action { get; }

    public BitArrayEnumerable<InputController, Keys> CurrentlyPressedKeys => _pressedKeys.AsEnumerable();
    public BitArrayEnumerable<InputController, Keys> CurrentlyReleasedKeys => _releasedKeys.AsEnumerable();

    public InputController()
    {
        _pressedKeys = new BitArraySet<InputController, KeysBitBuffer, Keys>(this);
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
        var inputActionsSpan = _inputActions.AsSpan();
        this.AssertUniqueIds(inputActionsSpan);
        inputActionsSpan.Sort(this);
    }

    public void Tick()
    {
        var inputActionsSpan = _inputActions.AsSpan();

        foreach (var keyAction in inputActionsSpan)
        {
            keyAction.UpdateState();
        }

        var keyMappingSpan = _keyMapping.AsSpan();

        foreach (var keyToInputMapping in keyMappingSpan)
        {
            if (_pressedKeys.Contains(keyToInputMapping.Key))
            {
                keyToInputMapping.InputAction.DoPress();
            }
        }

        UpdateKeyStates();
        UpdateMouseButtonStates();
    }

    public void ClearAllInputActions()
    {
        _pressedKeys.Clear();
        _releasedKeys.Clear();

        var inputActionsSpan = _inputActions.AsSpan();

        foreach (var inputAction in inputActionsSpan)
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
        void* p = &keyBoardState;
        var keysSpan = new ReadOnlySpan<uint>(p, KeysBitBuffer.KeysBitBufferLength);

        _releasedKeys.SetFrom(_pressedKeys);
        _pressedKeys.ReadFrom(keysSpan);

        _releasedKeys.ExceptWith(_pressedKeys);
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
