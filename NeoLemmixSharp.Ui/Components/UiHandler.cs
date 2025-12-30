using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Ui.Components;

public sealed class UiHandler : IDisposable
{
    private readonly InputController _inputController;

    private Component? _currentSelection = null;

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

        if (_currentSelection is null || !_currentSelection.IsVisible)
        {
            _currentSelection = RootComponent;
        }
        else
        {
            _currentSelection.InvokeMouseDoubleClick(mousePosition);
        }
    }

    private void HandleMouseDown(Point mousePosition)
    {
        LocateComponent(mousePosition);

        if (_currentSelection is null || !_currentSelection.IsVisible)
        {
            _currentSelection = RootComponent;
        }
        else
        {
            _currentSelection.InvokeMouseDown(mousePosition);
        }
    }

    private void HandleMouseUp(Point mousePosition)
    {
        LocateComponent(mousePosition);

        if (_currentSelection is null || !_currentSelection.IsVisible)
        {
            _currentSelection = RootComponent;
        }
        else
        {
            _currentSelection.InvokeMouseUp(mousePosition);
        }
    }

    private void HandleKeyDown(in KeysEnumerable pressedKeys)
    {
        _currentSelection?.InvokeKeyDown(in pressedKeys);
    }

    private void HandleKeyUp(in KeysEnumerable releasedKeys)
    {
        _currentSelection?.InvokeKeyUp(in releasedKeys);
    }

    private void LocateComponent(Point mousePosition)
    {
        var c = RootComponent.GetChildAt(mousePosition);

        if (c == null)
        {
            _currentSelection = RootComponent;
            return;
        }

        if (c == _currentSelection)
        {
            _currentSelection.InvokeMouseMovement(mousePosition);
            return;
        }

        if (_currentSelection == null)
            return;

        _currentSelection.InvokeMouseExit(mousePosition);

        _currentSelection = c;
        _currentSelection.InvokeMouseEnter(mousePosition);

        if (!_currentSelection.IsVisible)
        {
            _currentSelection = RootComponent;
        }
    }

    public void Dispose()
    {
        RootComponent.Dispose();
        GC.SuppressFinalize(this);
    }
}
