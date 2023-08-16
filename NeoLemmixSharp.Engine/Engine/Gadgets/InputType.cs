namespace NeoLemmixSharp.Engine.Engine.Gadgets;

public enum InputType
{
    Activate,
    Deactivate,

    MoveHorizontally,
    MoveVertically,

    ResizeHorizontally,
    ResizeVertically,

    LemmingEnterHitBox,
    LemmingWithinHitBox,
    LemmingExitHitBox
}