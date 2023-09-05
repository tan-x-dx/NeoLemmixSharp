using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class KeyAction : IIdEquatable<KeyAction>
{
    private const int EnabledMask = 3;

    private readonly string _actionName;
    private int _enabledMask;
    private int _keyState;

    public int Id { get; set; }

    public int KeyState
    {
        get => _keyState;
        set => _keyState = value & _enabledMask;
    }

    public KeyAction(string actionName)
    {
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

    public bool Equals(KeyAction? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is KeyAction other && Id == other.Id;
    public override int GetHashCode() => Id;

    public override string ToString() => _actionName;

    public static bool operator ==(KeyAction left, KeyAction right) => left.Id == right.Id;
    public static bool operator !=(KeyAction left, KeyAction right) => left.Id != right.Id;
}