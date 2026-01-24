namespace NeoLemmixSharp.Ui.Components.Buttons;

public sealed class Button : Component
{
    public Button(int x, int y, int width, int height, string? label)
        : base(x, y, width, height, label)
    {
        MouseEnter.RegisterMouseEvent(SetMouseOver);
        MousePressed.RegisterMouseEvent(SetMousePress);
        MouseReleased.RegisterMouseEvent(SetMouseOver);
        MouseExit.RegisterMouseEvent(SetMouseNormal);
    }

    public Button(int x, int y, string? label)
        : base(x, y, label)
    {
        MouseEnter.RegisterMouseEvent(SetMouseOver);
        MousePressed.RegisterMouseEvent(SetMousePress);
        MouseReleased.RegisterMouseEvent(SetMouseOver);
        MouseExit.RegisterMouseEvent(SetMouseNormal);
    }
}
