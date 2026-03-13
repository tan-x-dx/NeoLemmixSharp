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
    private readonly List<Component> _componentsThatShouldBeTicked = [];
    private TextField? _selectedTextField;
    private PopupMenu? _currentMenu;

    internal InputController InputController => _inputController;

    public Component RootComponent { get; }
    public Component CurrentSelection { get; private set; }

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
        private set
        {
            if (_currentMenu == value)
                return;

            var currentMenu = _currentMenu;
            _currentMenu = value;
            currentMenu?.CloseMenu();
        }
    }

    public UiHandler(InputController inputController)
    {
        RootComponent = new Root();
        CurrentSelection = RootComponent;
        _inputController = inputController;

        Instance = this;
    }

    public void Render(SpriteBatch spriteBatch)
    {
        RootComponent.Render(spriteBatch);
        CurrentMenu?.Render(spriteBatch);

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

        if (_componentsThatShouldBeTicked.Count > 0)
        {
            TickComponents();
        }
    }

    private void HandleMouseMove(Point mousePosition)
    {
        var component = LocateComponent(mousePosition);

        if (component == CurrentSelection)
        {
            component.InvokeMouseMovement(mousePosition);
            return;
        }

        CurrentSelection.InvokeMouseExit(mousePosition);

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
            CurrentSelection = RootComponent;
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
            CurrentSelection = RootComponent;
            return;
        }

        if (mouseButtonType == MouseButtonType.Left)
            TrySelectTextField(selectedComponent, mousePosition);

        selectedComponent.InvokeMousePressed(mousePosition, mouseButtonType);
    }

    private void HandleMouseHeld(Point mousePosition, MouseButtonType mouseButtonType)
    {
        var selectedComponent = CurrentSelection;
        if (selectedComponent is null || !selectedComponent.IsVisible)
        {
            CurrentSelection = RootComponent;
            return;
        }

        if (mouseButtonType == MouseButtonType.Left)
            TrySelectTextField(selectedComponent, mousePosition);

        selectedComponent.InvokeMouseHeld(mousePosition, mouseButtonType);
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
            CurrentSelection = RootComponent;
        }
        else
        {
            selectedComponent.InvokeMouseReleased(mousePosition, mouseButtonType);
        }
    }

    private void HandleKeyPressed(in KeysEnumerable pressedKeys)
    {
        var component = SelectedTextField ?? CurrentSelection;
        component.InvokeKeyPressed(in pressedKeys);
    }

    private void HandleKeyHeld(in KeysEnumerable heldKeys)
    {
        var component = SelectedTextField ?? CurrentSelection;
        component.InvokeKeyHeld(in heldKeys);
    }

    private void HandleKeyReleased(in KeysEnumerable releasedKeys)
    {
        CurrentSelection.InvokeKeyReleased(in releasedKeys);
    }

    [Pure]
    private Component LocateComponent(Point mousePosition)
    {
        var c = CurrentMenu ?? RootComponent;

        return c.GetChildAt(mousePosition) ?? c;
    }

    private void TickComponents()
    {
        foreach (var component in _componentsThatShouldBeTicked)
        {
            component.Tick();
        }
    }

    internal void DeselectTextField()
    {
        SelectedTextField = null;
    }

    public void OpenPopupMenu(PopupMenu popupMenu)
    {
        CurrentMenu = popupMenu;
    }

    public void ClosePopupMenu()
    {
        CurrentMenu = null;
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

    public void RegisterComponentForTicking(Component component)
    {
        if (_componentsThatShouldBeTicked.Contains(component))
            return;
        _componentsThatShouldBeTicked.Add(component);
    }

    public void DeregisterComponentForTicking(Component component)
    {
        _componentsThatShouldBeTicked.Remove(component);
    }

    public void Dispose()
    {
        _componentsThatShouldBeTicked.Clear();
        _menuFontTextLabels.Clear();

        SelectedTextField = null;
        CurrentMenu = null;
        CurrentSelection = RootComponent;
        RootComponent.Dispose();
        Instance = null!;
        GC.SuppressFinalize(this);
    }

    internal void EliminateComponentReferences(Component component)
    {
        DeregisterComponentForTicking(component);
        if (component is TextLabel textLabel)
            DeregisterTextLabelForShaderRendering(textLabel);

        if (component == SelectedTextField)
            SelectedTextField = null;

        if (component == CurrentMenu)
            CurrentMenu = null;

        if (component == CurrentSelection)
            CurrentSelection = RootComponent;
    }

    private sealed class Root : Component
    {
        public Root() : base(0, 0, 0, 0) { }
    }
}
