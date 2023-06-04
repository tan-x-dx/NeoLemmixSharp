using System;

namespace NeoLemmixSharp.Util;

public sealed class KeyAction : IEquatable<KeyAction>
{
    public int Id { get; }
    public int KeyState { get; set; }

    public KeyAction(int id)
    {
        Id = id;
    }

    public bool IsKeyDown => (KeyState & KeyStatusConsts.KeyPressed) == KeyStatusConsts.KeyPressed;
    public bool IsKeyUp => (KeyState & KeyStatusConsts.KeyReleased) == KeyStatusConsts.KeyReleased;
    public bool IsPressed => KeyState == KeyStatusConsts.KeyPressed;
    public bool IsReleased => KeyState == KeyStatusConsts.KeyReleased;
    public bool IsHeld => KeyState == KeyStatusConsts.KeyHeld;

    public bool Equals(KeyAction? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is KeyAction other && Equals(other);
    public override int GetHashCode() => Id;

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
