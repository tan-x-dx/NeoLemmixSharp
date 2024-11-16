using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Ui.Components;

public sealed class UiHandler
{
    private readonly InputController _inputController;

    private Component? _currentSelection = null;
    private bool _isInitialised = false;

    public Component RootComponent { get; set; }

    public UiHandler(
        int x,
        int y,
        int width,
        int height,
        InputController inputController)
    {
        RootComponent = new Tab(x, y, width, height);
        _inputController = inputController;
    }

    public bool IsInitialised() => _isInitialised;

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
    }

    private void HandleMouseMove(LevelPosition mousePosition) => LocateComponent(mousePosition);

    private void HandleMouseDown(LevelPosition mousePosition)
    {
        LocateComponent(mousePosition);

        if (_currentSelection!.Visible)
        {
            _currentSelection.InvokeMouseDown(mousePosition);
        }
        else
        {
            _currentSelection = RootComponent;
        }
    }

    private void HandleMouseUp(LevelPosition mousePosition)
    {
        LocateComponent(mousePosition);

        if (_currentSelection!.Visible)
        {
            _currentSelection.InvokeMouseUp(mousePosition);
        }
        else
        {
            _currentSelection = RootComponent;
        }
    }

    private void HandleKeyDown()
    {
        _currentSelection?.InvokeKeyDown();
    }

    private void HandleKeyUp()
    {
        _currentSelection?.InvokeKeyUp();
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
}
