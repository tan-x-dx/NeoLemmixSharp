namespace NeoLemmixSharp.Ui.Components;

public sealed class PopupMenu : Component
{
    public bool DisposeOnClose { get; set; } = true;

    public PopupMenu()
    {
        MousePressed.RegisterMousePressEvent(OnClickOutsideWindowBounds, MouseButtonType.Left);
        MousePressed.RegisterMousePressEvent(OnClickOutsideWindowBounds, MouseButtonType.Right);
    }

    internal void CloseMenu()
    {
        if (DisposeOnClose)
            Dispose();
    }

    private void OnClickOutsideWindowBounds(Component c, Common.Point position)
    {
        if (ContainsPoint(position))
            return;

        UiHandler.Instance.ClosePopupMenu();
    }
}
