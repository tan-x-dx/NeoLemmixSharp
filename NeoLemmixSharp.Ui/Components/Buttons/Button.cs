namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class Button : Component
{
    public Button(int x, int y, int width, int height)
        : base(x, y, width, height)
    {
        MouseEnter.RegisterMouseMoveEvent(SetMouseOver);
        MousePressed.RegisterMousePressEvent(SetMousePress, MouseButtonType.Left);
        MouseReleased.RegisterMousePressEvent(SetMouseOver, MouseButtonType.Left);
        MouseExit.RegisterMouseMoveEvent(SetMouseNormal);
    }

    public Button(int x, int y)
        : base(x, y)
    {
        MouseEnter.RegisterMouseMoveEvent(SetMouseOver);
        MousePressed.RegisterMousePressEvent(SetMousePress, MouseButtonType.Left);
        MouseReleased.RegisterMousePressEvent(SetMouseOver, MouseButtonType.Left);
        MouseExit.RegisterMouseMoveEvent(SetMouseNormal);
    }
}
