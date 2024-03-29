﻿using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu;

public sealed class MenuInputController
{
    private readonly InputController _inputController = new();

    public int MouseX => _inputController.MouseX;
    public int MouseY => _inputController.MouseY;
    public int ScrollDelta => _inputController.ScrollDelta;

    public InputAction LeftMouseButtonAction => _inputController.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => _inputController.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => _inputController.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => _inputController.MouseButton4Action;
    public InputAction MouseButton5Action => _inputController.MouseButton5Action;

    public InputAction F1 { get; }
    public InputAction F2 { get; }
    public InputAction F3 { get; }

    public InputAction RightArrow { get; }
    public InputAction UpArrow { get; }
    public InputAction LeftArrow { get; }
    public InputAction DownArrow { get; }

    public InputAction Space { get; }
    public InputAction Enter { get; }

    public InputAction ToggleFullScreen { get; }
    public InputAction Quit { get; }

    public MenuInputController()
    {
        F1 = _inputController.CreateInputAction("F1");
        F2 = _inputController.CreateInputAction("F2");
        F3 = _inputController.CreateInputAction("F3");

        RightArrow = _inputController.CreateInputAction("\u2192");
        UpArrow = _inputController.CreateInputAction("\u2191");
        LeftArrow = _inputController.CreateInputAction("\u2190");
        DownArrow = _inputController.CreateInputAction("\u2193");

        Space = _inputController.CreateInputAction("Space");
        Enter = _inputController.CreateInputAction("Enter");

        ToggleFullScreen = _inputController.CreateInputAction("Toggle Full Screen");
        Quit = _inputController.CreateInputAction("Quit");

        _inputController.ValidateInputActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        _inputController.Bind(Keys.F1, F1);
        _inputController.Bind(Keys.F2, F2);
        _inputController.Bind(Keys.F3, F3);

        _inputController.Bind(Keys.A, LeftArrow);
        _inputController.Bind(Keys.W, UpArrow);
        _inputController.Bind(Keys.D, RightArrow);
        _inputController.Bind(Keys.S, DownArrow);

        _inputController.Bind(Keys.Left, LeftArrow);
        _inputController.Bind(Keys.Up, UpArrow);
        _inputController.Bind(Keys.Right, RightArrow);
        _inputController.Bind(Keys.Down, DownArrow);

        _inputController.Bind(Keys.Space, Space);
        _inputController.Bind(Keys.Enter, Enter);

        _inputController.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
        _inputController.Bind(Keys.Escape, Quit);
    }

    public void ClearAllInputActions() => _inputController.ClearAllInputActions();

    public void Tick() => _inputController.Tick();
}