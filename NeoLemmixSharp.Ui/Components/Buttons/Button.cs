using NeoLemmixSharp.Ui.Data;

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

    public TextLabel AddTextLabel(string text) => AddTextLabel(text, UiConstants.DefaultTextYOffset, UiConstants.DefaultTextYOffset, UiConstants.AllBlackColors);

    public TextLabel AddTextLabel(string text, int labelOffsetX, int labelOffsetY, ColorPacket colorPacket)
    {
        var textLabel = new TextLabel(text)
        {
            Left = 0,
            Top = 0,
            LabelOffsetX = labelOffsetX,
            LabelOffsetY = labelOffsetY,
            Colors = colorPacket
        };
        AddChild(textLabel);
        return textLabel;
    }
}
