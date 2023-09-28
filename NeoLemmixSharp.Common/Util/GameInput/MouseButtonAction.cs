using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.Identity;

namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class MouseButtonAction : IIdEquatable<MouseButtonAction>
{
    private readonly string _actionName;
    public int Id { get; }
    public int MouseButtonState { get; private set; }

    public MouseButtonAction(int id, string actionName)
    {
        Id = id;
        _actionName = actionName;
    }

    public void UpdateState(ButtonState mouseState)
    {
        MouseButtonState = ((MouseButtonState << 1) & 2) | (int)mouseState;
    }

    /// <summary>
    /// Is the Mouse Button currently pressed down?
    /// </summary>
    public bool IsMouseButtonDown => (MouseButtonState & MouseButtonStatusConsts.MouseButtonPressed) == MouseButtonStatusConsts.MouseButtonPressed;
    /// <summary>
    /// Is the Mouse Button currently released?
    /// </summary>
    public bool IsMouseButtonUp => (MouseButtonState & MouseButtonStatusConsts.MouseButtonReleased) == MouseButtonStatusConsts.MouseButtonUnpressed;
    /// <summary>
    /// Is the Mouse Button currently pressed down, but it was previously released?
    /// </summary>
    public bool IsPressed => MouseButtonState == MouseButtonStatusConsts.MouseButtonPressed;
    /// <summary>
    /// Is the Mouse Button currently released, but it was previously pressed down?
    /// </summary>
    public bool IsReleased => MouseButtonState == MouseButtonStatusConsts.MouseButtonReleased;
    /// <summary>
    /// Is the Mouse Button currently being pressed down and it was previously pressed down?
    /// </summary>
    public bool IsHeld => MouseButtonState == MouseButtonStatusConsts.MouseButtonHeld;

    public bool Equals(MouseButtonAction? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is MouseButtonAction other && Id == other.Id;
    public override int GetHashCode() => Id;

    public override string ToString() => _actionName;

    public static bool operator ==(MouseButtonAction left, MouseButtonAction right) => left.Id == right.Id;
    public static bool operator !=(MouseButtonAction left, MouseButtonAction right) => left.Id != right.Id;
}