using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Ui.Components;

public sealed class UiHandler : IDisposable
{
    public static UiHandler Instance { get; set; } = null!;

    private readonly InputController _inputController;
    private readonly List<TextLabel> _menuFontTextLabels = [];
    private TextField? _selectedTextField;
    private PopupMenu? _currentMenu;

    internal InputController InputController => _inputController;

    public Component? CurrentSelection { get; private set; }
    public TextField? SelectedTextField
    {
        get => _selectedTextField;
        private set
        {
            if (_selectedTextField == value)
                return;

            var currentTextField = _selectedTextField;
            if (currentTextField != null)
            {
                currentTextField.InvokeTextSubmit();
                currentTextField.Deselect();
            }

            _selectedTextField = value;
            _selectedTextField?.SetSelected();
        }
    }
    public PopupMenu? CurrentMenu
    {
        get => _currentMenu;
        set
        {
            if (_currentMenu == value)
                return;

            var currentMenu = _currentMenu;
            if (currentMenu != null)
            {
                currentMenu.CloseMenu();
            }

            _currentMenu = value;
        }
    }

    public Component RootComponent { get; }

    public UiHandler(InputController inputController)
    {
        RootComponent = new Root();
        _inputController = inputController;

        Instance = this;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        RootComponent.Render(spriteBatch);

        if (_menuFontTextLabels.Count == 0)
            return;
    }

    public bool HasMenuFontsToRender() => _menuFontTextLabels.Count > 0;

    public void RenderMenuFontText(SpriteBatch spriteBatch)
    {
        foreach (var label in _menuFontTextLabels)
        {
            label.RenderMenuFont(spriteBatch);
        }
    }

    public void Tick()
    {
        var mousePosition = _inputController.MousePosition;
        HandleMouseMove(mousePosition);

        HandleMouseInteraction(mousePosition, _inputController.LeftMouseButtonAction, MouseButtonType.Left);
        HandleMouseInteraction(mousePosition, _inputController.MiddleMouseButtonAction, MouseButtonType.Middle);
        HandleMouseInteraction(mousePosition, _inputController.RightMouseButtonAction, MouseButtonType.Right);
        HandleMouseInteraction(mousePosition, _inputController.MouseButton4Action, MouseButtonType.Mouse4);
        HandleMouseInteraction(mousePosition, _inputController.MouseButton5Action, MouseButtonType.Mouse5);

        var currentlyPressedKeys = _inputController.CurrentlyPressedKeys;
        var currentlyHeldKeys = _inputController.CurrentlyHeldKeys;
        var justReleasedKeys = _inputController.JustReleasedKeys;

        HandleKeyPressed(in currentlyPressedKeys);
        HandleKeyHeld(in currentlyHeldKeys);
        HandleKeyReleased(in justReleasedKeys);
    }

    private void HandleMouseMove(Point mousePosition)
    {
        var component = LocateComponent(mousePosition);

        if (component == null)
        {
            CurrentSelection = RootComponent;
            return;
        }

        if (component == CurrentSelection)
        {
            component.InvokeMouseMovement(mousePosition);
            return;
        }

        CurrentSelection?.InvokeMouseExit(mousePosition);

        CurrentSelection = component;
        component.InvokeMouseEnter(mousePosition);

        if (!component.IsVisible)
        {
            CurrentSelection = RootComponent;
        }
    }

    private void HandleMouseInteraction(Point mousePosition, InputAction mouseButton, MouseButtonType mouseButtonType)
    {
        if (mouseButton.IsDoubleTap)
        {
            HandleMouseDoubleClick(mousePosition, mouseButtonType);
        }
        if (mouseButton.IsPressed)
        {
            HandleMousePress(mousePosition, mouseButtonType);
        }
        else if (mouseButton.IsHeld)
        {
            HandleMouseHeld(mousePosition, mouseButtonType);
        }
        else if (mouseButton.IsReleased)
        {
            HandleMouseRelease(mousePosition, mouseButtonType);
        }
    }

    private void HandleMouseDoubleClick(Point mousePosition, MouseButtonType mouseButtonType)
    {
        var selectedComponent = CurrentSelection;
        if (selectedComponent is null || !selectedComponent.IsVisible)
        {
            selectedComponent = RootComponent;
        }
        else
        {
            selectedComponent.InvokeMouseDoubleClick(mousePosition, mouseButtonType);
        }
    }

    private void HandleMousePress(Point mousePosition, MouseButtonType mouseButtonType)
    {
        var selectedComponent = CurrentSelection;
        if (selectedComponent is null || !selectedComponent.IsVisible)
        {
            selectedComponent = RootComponent;
        }
        else
        {
            if (mouseButtonType == MouseButtonType.Left)
            {
                TrySelectTextField(selectedComponent, mousePosition);
                SelectedTextField = selectedComponent as TextField;
            }
            selectedComponent.InvokeMousePressed(mousePosition, mouseButtonType);
        }
    }

    private void HandleMouseHeld(Point mousePosition, MouseButtonType mouseButtonType)
    {
        var selectedComponent = CurrentSelection;
        if (selectedComponent is null || !selectedComponent.IsVisible)
        {
            selectedComponent = RootComponent;
        }
        else
        {
            if (mouseButtonType == MouseButtonType.Left)
            {
                TrySelectTextField(selectedComponent, mousePosition);
                SelectedTextField = selectedComponent as TextField;
            }
            selectedComponent.InvokeMouseHeld(mousePosition, mouseButtonType);
        }
    }

    private void TrySelectTextField(Component selectedComponent, Point mousePosition)
    {
        SelectedTextField = selectedComponent as TextField;
        SelectedTextField?.SetCaretPositionFromMousePosition(mousePosition);
    }

    private void HandleMouseRelease(Point mousePosition, MouseButtonType mouseButtonType)
    {
        var selectedComponent = CurrentSelection;
        if (selectedComponent is null || !selectedComponent.IsVisible)
        {
            selectedComponent = RootComponent;
        }
        else
        {
            selectedComponent.InvokeMouseReleased(mousePosition, mouseButtonType);
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

    [Pure]
    private Component? LocateComponent(Point mousePosition)
    {
        var c = CurrentMenu ?? RootComponent;

        return c.GetChildAt(mousePosition);
    }

    internal void DeselectTextField()
    {
        SelectedTextField = null;
    }

    internal void RegisterTextLabelForShaderRendering(TextLabel textLabel)
    {
        if (_menuFontTextLabels.Contains(textLabel))
            return;
        _menuFontTextLabels.Add(textLabel);
    }

    internal void DeregisterTextLabelForShaderRendering(TextLabel textLabel)
    {
        _menuFontTextLabels.Remove(textLabel);
    }

    public void Dispose()
    {
        RootComponent.Dispose();
        Instance = null!;
        GC.SuppressFinalize(this);
    }

    private sealed class Root : Component
    {
        public Root() : base(0, 0, 0, 0) { }
    }
}
