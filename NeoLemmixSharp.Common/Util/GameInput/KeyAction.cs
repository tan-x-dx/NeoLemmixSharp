using NeoLemmixSharp.Common.Util.Identity;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class KeyAction : IIdEquatable<KeyAction>, IInputAction
{
    private readonly InputAction _action;
    private readonly string _actionName;

    public int Id { get; }

    public KeyAction(int id, string actionName)
    {
        Id = id;
        _actionName = actionName;
        _action = new InputAction();
    }

    public ulong ActionState
    {
        get => _action.ActionState;
        set => _action.ActionState = value;
    }

    public void UpdateState()
    {
        _action.UpdateState();
    }

    public void SetEnabled(bool enable)
    {
        _action.SetEnabled(enable);
    }

    public bool IsActionDown => _action.IsActionDown;

    public bool IsActionUp => _action.IsActionUp;

    public bool IsPressed => _action.IsPressed;

    public bool IsReleased => _action.IsReleased;

    public bool IsHeld => _action.IsHeld;

    public bool IsDoubleTap => _action.IsDoubleTap;

    public bool IsEnabled => _action.IsEnabled;

    public bool Equals(KeyAction? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is KeyAction other && Id == other.Id;
    public override int GetHashCode() => Id;

    public override string ToString() => _actionName;

    public static bool operator ==(KeyAction left, KeyAction right) => left.Id == right.Id;
    public static bool operator !=(KeyAction left, KeyAction right) => left.Id != right.Id;
}