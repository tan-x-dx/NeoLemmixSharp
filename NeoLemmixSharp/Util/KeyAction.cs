using System;

namespace NeoLemmixSharp.Util;

public sealed class KeyAction : IEquatable<KeyAction>
{
    private readonly string _actionName;
    public int Id { get; }
    public int KeyState { get; set; }

    public KeyAction(int id, string actionName)
    {
        Id = id;
        _actionName = actionName;
    }

    /// <summary>
    /// Is the Key currently pressed down?
    /// </summary>
    public bool IsKeyDown => (KeyState & KeyStatusConsts.KeyPressed) == KeyStatusConsts.KeyPressed;
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

    public bool Equals(KeyAction? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is KeyAction other && Id == other.Id;
    public override int GetHashCode() => Id;

    public override string ToString() => _actionName;

    public static bool operator ==(KeyAction? left, KeyAction? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.Id == right.Id;
    }

    public static bool operator !=(KeyAction? left, KeyAction? right) => !(left == right);
}
