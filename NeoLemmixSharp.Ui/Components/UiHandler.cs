using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Ui.Components;

public sealed class UiHandler : IDisposable
{
    private readonly InputController _inputController;

    public Component? CurrentSelection { get; private set; } = null;

    public Component RootComponent { get; set; }

    public UiHandler(
        InputController inputController)
    {
        RootComponent = new Tab(0, 0, 0, 0);
        _inputController = inputController;
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
            HandleMouseDown(mousePosition);
        }
        else if (leftMouseButton.IsReleased)
        {
            HandleMouseUp(mousePosition);
        }

        var currentlyPressedKeys = _inputController.CurrentlyPressedKeys;
        var currentlyReleasedKeys = _inputController.CurrentlyReleasedKeys;

        HandleKeyDown(in currentlyPressedKeys);
        HandleKeyUp(in currentlyReleasedKeys);
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

    private void HandleMouseDown(Point mousePosition)
    {
        LocateComponent(mousePosition);

        if (CurrentSelection is null || !CurrentSelection.IsVisible)
        {
            CurrentSelection = RootComponent;
        }
        else
        {
            CurrentSelection.InvokeMouseDown(mousePosition);
        }
    }

    private void HandleMouseUp(Point mousePosition)
    {
        LocateComponent(mousePosition);

        if (CurrentSelection is null || !CurrentSelection.IsVisible)
        {
            CurrentSelection = RootComponent;
        }
        else
        {
            CurrentSelection.InvokeMouseUp(mousePosition);
        }
    }

    private void HandleKeyDown(in KeysEnumerable pressedKeys)
    {
        CurrentSelection?.InvokeKeyDown(in pressedKeys);
    }

    private void HandleKeyUp(in KeysEnumerable releasedKeys)
    {
        CurrentSelection?.InvokeKeyUp(in releasedKeys);
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

    public void Dispose()
    {
        RootComponent.Dispose();
        GC.SuppressFinalize(this);
    }
}
