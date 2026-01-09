using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu;

public sealed class MenuInputController
{
    public InputController InputController { get; } = new();

    public Point MousePosition => InputController.MousePosition;
    public int ScrollDelta => InputController.ScrollDelta;

    public InputAction LeftMouseButtonAction => InputController.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => InputController.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => InputController.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => InputController.MouseButton4Action;
    public InputAction MouseButton5Action => InputController.MouseButton5Action;

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
        F1 = InputController.CreateInputAction("F1");
        F2 = InputController.CreateInputAction("F2");
        F3 = InputController.CreateInputAction("F3");

        RightArrow = InputController.CreateInputAction("\u2192");
        UpArrow = InputController.CreateInputAction("\u2191");
        LeftArrow = InputController.CreateInputAction("\u2190");
        DownArrow = InputController.CreateInputAction("\u2193");

        Space = InputController.CreateInputAction("Space");
        Enter = InputController.CreateInputAction("Enter");

        ToggleFullScreen = InputController.CreateInputAction("Toggle Full Screen");
        Quit = InputController.CreateInputAction("Quit");

        InputController.ValidateInputActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        InputController.Bind(Keys.F1, F1);
        InputController.Bind(Keys.F2, F2);
        InputController.Bind(Keys.F3, F3);

        InputController.Bind(Keys.A, LeftArrow);
        InputController.Bind(Keys.W, UpArrow);
        InputController.Bind(Keys.D, RightArrow);
        InputController.Bind(Keys.S, DownArrow);

        InputController.Bind(Keys.Left, LeftArrow);
        InputController.Bind(Keys.Up, UpArrow);
        InputController.Bind(Keys.Right, RightArrow);
        InputController.Bind(Keys.Down, DownArrow);

        InputController.Bind(Keys.Space, Space);
        InputController.Bind(Keys.Enter, Enter);

        InputController.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
        InputController.Bind(Keys.Escape, Quit);
    }

    public void ClearAllInputActions() => InputController.ClearAllInputActions();

    public void Tick() => InputController.Tick();
}
