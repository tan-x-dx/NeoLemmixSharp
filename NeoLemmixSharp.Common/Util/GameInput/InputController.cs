using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.GameInput;

public abstract class InputController : ISimpleHasher<Keys>, IComparer<KeyAction>
{
    private const int NumberOfKeys = 256;

    private readonly List<(Keys, KeyAction)> _keyMapping = new();
    private readonly LargeSimpleSet<Keys> _keys;
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

    protected InputController()
    {
        _keys = new LargeSimpleSet<Keys>(this);

        // ReSharper disable once VirtualMemberCallInConstructor
        SetUpBindings();
        ValidateKeyActions();
    }

    public void Tick()
    {
        for (var i = 0; i < _keyActions.Count; i++)
        {
            _keyActions[i].UpdateStatus();
        }

        for (var index = 0; index < _keyMapping.Count; index++)
        {
            var (keyValue, action) = _keyMapping[index];
            if (_keys.Contains(keyValue))
            {
                _keyActions[action.Id].KeyState |= KeyStatusConsts.KeyPressed;
            }
        }

        UpdateKeyStates();
        UpdateMouseButtonStates();
    }

    protected abstract void SetUpBindings();

    protected void Bind(Keys keyCode, KeyAction keyAction)
    {
        _keyMapping.Add((keyCode, keyAction));
        keyAction.Id = _keyActions.Count;
        _keyActions.Add(keyAction);
    }

    private void ValidateKeyActions()
    {
        _keyActions.ValidateUniqueIds();
        _keyActions.Sort(this);
    }

    public void ReleaseAllKeys()
    {
        _keys.Clear();
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

        LeftMouseButtonAction.UpdateState(mouseState.LeftButton);
        RightMouseButtonAction.UpdateState(mouseState.RightButton);
        MiddleMouseButtonAction.UpdateState(mouseState.MiddleButton);

        MouseButton4Action.UpdateState(mouseState.XButton1);
        MouseButton5Action.UpdateState(mouseState.XButton2);
    }

    int IComparer<KeyAction>.Compare(KeyAction? x, KeyAction? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        return x.Id.CompareTo(y.Id);
    }

    bool IEquatable<ISimpleHasher<Keys>>.Equals(ISimpleHasher<Keys>? other) => ReferenceEquals(this, other);
    int ISimpleHasher<Keys>.NumberOfItems => NumberOfKeys;
    int ISimpleHasher<Keys>.Hash(Keys item) => (int)item;
    Keys ISimpleHasher<Keys>.Unhash(int index) => (Keys)index;
}