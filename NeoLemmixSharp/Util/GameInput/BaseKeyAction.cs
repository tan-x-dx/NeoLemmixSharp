using System;

namespace NeoLemmixSharp.Util.GameInput;

public abstract class BaseKeyAction : IEquatable<BaseKeyAction>
{
    private const int EnabledMask = 3;

    private readonly string _actionName;
    private int _enabledMask;
    private int _keyState;

    public int Id { get; }

    public int KeyState
    {
        get => _keyState;
        set => _keyState = value & _enabledMask;
    }

    protected BaseKeyAction(int id, string actionName)
    {
        Id = id;
        _enabledMask = EnabledMask;
        _actionName = actionName;
    }

    public void UpdateStatus()
    {
        _keyState = (_keyState << 1) & _enabledMask;
    }

    public void SetEnabled(bool enable)
    {
        _enabledMask = enable ? EnabledMask : 0;
    }

    /// <summary>
    /// Is the Key currently pressed down?
    /// </summary>
    public bool IsKeyDown => (KeyState & KeyStatusConsts.KeyPressed) == KeyStatusConsts.KeyPressed;
    /// <summary>
    /// Is the Key currently released?
    /// </summary>
    public bool IsKeyUp => (KeyState & KeyStatusConsts.KeyReleased) == KeyStatusConsts.KeyUnpressed;
    /// <summary>
    /// Is the Key currently pressed down, but it was previously released?
    /// </summary>
    public bool IsPressed => KeyState == KeyStatusConsts.KeyPressed;
    /// <summary>
    /// Is the Key currently released, but it was previously pressed down?
    /// </summary>
    public bool IsReleased => KeyState == KeyStatusConsts.KeyReleased;
    /// <summary>
    /// Is the Key currently being pressed down and it was previously pressed down?
    /// </summary>
    public bool IsHeld => KeyState == KeyStatusConsts.KeyHeld;

    public bool IsEnabled => _enabledMask != 0;

    public bool Equals(BaseKeyAction? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is BaseKeyAction other && Id == other.Id;
    public override int GetHashCode() => Id;

    public override string ToString() => _actionName;

    public static bool operator ==(BaseKeyAction? left, BaseKeyAction? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Id == right.Id;
    }

    public static bool operator !=(BaseKeyAction? left, BaseKeyAction? right) => !(left == right);
}