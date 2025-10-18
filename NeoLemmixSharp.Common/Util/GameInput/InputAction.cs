using NeoLemmixSharp.Common.Util.Identity;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class InputAction : IIdEquatable<InputAction>
{
    private const ulong EnabledMask = (1UL << EngineConstants.EngineTicksPerSecond) - 1UL;
    private const ulong DisabledMask = 0UL;

    private const ulong ActionUnpressed = 0UL;
    private const ulong ActionPressed = 1UL;
    private const ulong ActionReleased = 2UL;
    private const ulong ActionHeld = 3UL;
    private const ulong DoubleTapUpperMask = ((1UL << (EngineConstants.DoubleTapFrameCountMax - 2)) - 1UL) << 2;

    private readonly string _actionName;
    private ulong _stateMask = EnabledMask;
    private ulong _actionState;

    public int Id { get; }

    public InputAction(int id, string actionName)
    {
        Id = id;
        _actionName = actionName;
    }

    public ulong ActionState
    {
        get => _actionState;
        set => _actionState = value & _stateMask;
    }

    public void Clear()
    {
        _actionState = 0UL;
    }

    public void UpdateState()
    {
        _actionState = (_actionState << 1) & _stateMask;
    }

    public void SetEnabled(bool enable)
    {
        _stateMask = enable ? EnabledMask : DisabledMask;
    }

    /// <summary>
    /// Is the Action currently pressed down?
    /// </summary>
    public bool IsActionDown => (_actionState & ActionPressed) == ActionPressed;
    /// <summary>
    /// Is the Action currently released?
    /// </summary>
    public bool IsActionUp => (_actionState & ActionPressed) == ActionUnpressed;
    /// <summary>
    /// Is the Action currently pressed down, but it was previously released?
    /// </summary>
    public bool IsPressed => (_actionState & ActionHeld) == ActionPressed;
    /// <summary>
    /// Is the Action currently released, but it was previously pressed down?
    /// </summary>
    public bool IsReleased => (_actionState & ActionHeld) == ActionReleased;
    /// <summary>
    /// Is the Action currently being pressed down, and it was previously pressed down?
    /// </summary>
    public bool IsHeld => (_actionState & ActionHeld) == ActionHeld;
    /// <summary>
    /// Has a double tap occurred for the Action? Double tap has about 1/4 of a second to take place.
    /// </summary>
    public bool IsDoubleTap => (_actionState & ActionPressed) != 0UL &&
                               (_actionState & ActionReleased) == 0UL &&
                               (_actionState & DoubleTapUpperMask) != 0UL;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DoPress()
    {
        ActionState |= ActionPressed;
    }

    public bool IsEnabled => _stateMask != DisabledMask;

    [DebuggerStepThrough]
    public bool Equals(InputAction? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }

    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is InputAction other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;

    [DebuggerStepThrough]
    public override string ToString() => _actionName;

    [DebuggerStepThrough]
    public static bool operator ==(InputAction left, InputAction right) => left.Id == right.Id;
    [DebuggerStepThrough]
    public static bool operator !=(InputAction left, InputAction right) => left.Id != right.Id;
}
