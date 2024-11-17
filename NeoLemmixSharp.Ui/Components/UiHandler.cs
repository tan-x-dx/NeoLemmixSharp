using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
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
        var mousePosition = new LevelPosition(_inputController.MouseX, _inputController.MouseY);
        HandleMouseMove(mousePosition);

        var leftMouseButton = _inputController.LeftMouseButtonAction;

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

    private void HandleMouseMove(LevelPosition mousePosition) => LocateComponent(mousePosition);

    private void HandleMouseDown(LevelPosition mousePosition)
    {
        LocateComponent(mousePosition);

        if (_currentSelection is null || !_currentSelection.Visible)
        {
            _currentSelection = RootComponent;
        }
        else
        {
            _currentSelection.InvokeMouseDown(mousePosition);
        }
    }

    private void HandleMouseUp(LevelPosition mousePosition)
    {
        LocateComponent(mousePosition);

        if (_currentSelection is null || !_currentSelection.Visible)
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

    private void LocateComponent(LevelPosition mousePosition)
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

        if (!_currentSelection.Visible)
        {
            _currentSelection = RootComponent;
        }
    }

    public void Dispose()
    {
        RootComponent.Dispose();
    }
}
