﻿using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class KeyAction : IIdEquatable<KeyAction>
{
    private const int EnabledMask = 3;
    private const int DisabledMask = 0;

    private readonly string _actionName;
    private int _stateMask;
    private int _keyState;

    public int Id { get; set; }

    public int KeyState
    {
        get => _keyState;
        set => _keyState = value & _stateMask;
    }

    public KeyAction(string actionName)
    {
        _actionName = actionName;
        _stateMask = EnabledMask;
    }

    public void UpdateStatus()
    {
        _keyState = (_keyState << 1) & _stateMask;
    }

    public void SetEnabled(bool enable)
    {
        _stateMask = enable ? EnabledMask : DisabledMask;
    }

    /// <summary>
    /// Is the Key currently pressed down?
    /// </summary>
    public bool IsKeyDown => (KeyState & KeyStatusConstants.KeyPressed) == KeyStatusConstants.KeyPressed;
    /// <summary>
    /// Is the Key currently released?
    /// </summary>
    public bool IsKeyUp => (KeyState & KeyStatusConstants.KeyReleased) == KeyStatusConstants.KeyUnpressed;
    /// <summary>
    /// Is the Key currently pressed down, but it was previously released?
    /// </summary>
    public bool IsPressed => KeyState == KeyStatusConstants.KeyPressed;
    /// <summary>
    /// Is the Key currently released, but it was previously pressed down?
    /// </summary>
    public bool IsReleased => KeyState == KeyStatusConstants.KeyReleased;
    /// <summary>
    /// Is the Key currently being pressed down and it was previously pressed down?
    /// </summary>
    public bool IsHeld => KeyState == KeyStatusConstants.KeyHeld;

    public bool IsEnabled => _stateMask != DisabledMask;

    public bool Equals(KeyAction? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is KeyAction other && Id == other.Id;
    public override int GetHashCode() => Id;

    public override string ToString() => _actionName;

    public static bool operator ==(KeyAction left, KeyAction right) => left.Id == right.Id;
    public static bool operator !=(KeyAction left, KeyAction right) => left.Id != right.Id;
}