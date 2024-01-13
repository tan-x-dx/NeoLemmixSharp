using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public interface IButtonAction
{
    ButtonType ButtonType { get; }

    void OnMouseDown();
    void OnPress(bool isDoubleTap);

    void OnRightClick();
}

public sealed class EmptyButtonAction : IButtonAction
{
    public static readonly EmptyButtonAction Instance = new();

    private EmptyButtonAction()
    {
    }

    public ButtonType ButtonType => ButtonType.Padding;

    public void OnMouseDown()
    {
    }

    public void OnPress(bool isDoubleTap)
    {
    }

    public void OnRightClick()
    {
    }
}