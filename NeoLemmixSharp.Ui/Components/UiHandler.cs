using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Ui.Components;

public sealed class UiHandler : IDisposable
{
    private readonly InputController _inputController;

    internal static UiHandler Instance { get; private set; } = null!;

    public Component? CurrentSelection { get; private set; }
    public TextField? SelectedTextField
    {
        get => field;
        private set
        {
            field?.IsSelected = false;
            field = value;
            field?.IsSelected = true;
        }
    }

    public Component RootComponent { get; set; }

    public UiHandler(InputController inputController)
    {
        RootComponent = new Tab(0, 0, 0, 0);
        _inputController = inputController;

        Instance = this;
    }

    public void Render(SpriteBatch spriteBatch) => RootComponent.Render(spriteBatch);

    public void Tick()
    {
        var mousePosition = _inputController.MousePosition;
        HandleMouseMove(mousePosition);

        var leftMouseButton = _inputController.LeftMouseButtonAction;

        if (leftMouseButton.IsDoubleTap)
        {
            HandleMouseDoubleClick(mousePosition);
        }
        if (leftMouseButton.IsPressed)
        {
            HandleMousePress(mousePosition);
        }
        else if (leftMouseButton.IsReleased)
        {
            HandleMouseRelease(mousePosition);
        }

        var currentlyPressedKeys = _inputController.CurrentlyPressedKeys;
        var currentlyHeldKeys = _inputController.CurrentlyHeldKeys;
        var justReleasedKeys = _inputController.JustReleasedKeys;

        HandleKeyPressed(in currentlyPressedKeys);
        HandleKeyHeld(in currentlyHeldKeys);
        HandleKeyReleased(in justReleasedKeys);
    }

    private void HandleMouseMove(Point mousePosition) => LocateComponent(mousePosition);

    private void HandleMouseDoubleClick(Point mousePosition)
    {
        LocateComponent(mousePosition);

        if (CurrentSelection is null || !CurrentSelection.IsVisible)
        {
            CurrentSelection = RootComponent;
        }
        else
        {
            CurrentSelection.InvokeMouseDoubleClick(mousePosition);
        }
    }

    private void HandleMousePress(Point mousePosition)
    {
        LocateComponent(mousePosition);

        if (CurrentSelection is null || !CurrentSelection.IsVisible)
        {
            CurrentSelection = RootComponent;
        }
        else
        {
            SelectedTextField = CurrentSelection as TextField;
            CurrentSelection.InvokeMousePressed(mousePosition);
        }
    }

    private void HandleMouseRelease(Point mousePosition)
    {
        LocateComponent(mousePosition);

        if (CurrentSelection is null || !CurrentSelection.IsVisible)
        {
            CurrentSelection = RootComponent;
        }
        else
        {
            CurrentSelection.InvokeMouseReleased(mousePosition);
        }
    }

    private void HandleKeyPressed(in KeysEnumerable pressedKeys)
    {
        var component = SelectedTextField ?? CurrentSelection;
        component?.InvokeKeyPressed(in pressedKeys);
    }

    private void HandleKeyHeld(in KeysEnumerable heldKeys)
    {
        var component = SelectedTextField ?? CurrentSelection;
        component?.InvokeKeyHeld(in heldKeys);
    }

    private void HandleKeyReleased(in KeysEnumerable releasedKeys)
    {
        CurrentSelection?.InvokeKeyReleased(in releasedKeys);
    }

    private void LocateComponent(Point mousePosition)
    {
        var c = RootComponent.GetChildAt(mousePosition);

        if (c == null)
        {
            CurrentSelection = RootComponent;
            return;
        }

        if (c == CurrentSelection)
        {
            CurrentSelection.InvokeMouseMovement(mousePosition);
            return;
        }

        if (CurrentSelection == null)
            return;

        CurrentSelection.InvokeMouseExit(mousePosition);

        CurrentSelection = c;
        CurrentSelection.InvokeMouseEnter(mousePosition);

        if (!CurrentSelection.IsVisible)
        {
            CurrentSelection = RootComponent;
        }
    }

    public void DeselectTextField()
    {
        SelectedTextField = null;
    }

    public void Dispose()
    {
        RootComponent.Dispose();
        Instance = null!;
        GC.SuppressFinalize(this);
    }
}
