using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed class LevelEditorController
{
    public InputHandler InputHandler { get; } = new();

    public Point MousePosition => InputHandler.MousePosition;
    public int ScrollDelta => InputHandler.ScrollDelta;

    public InputAction LeftMouseButtonAction => InputHandler.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => InputHandler.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => InputHandler.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => InputHandler.MouseButton4Action;
    public InputAction MouseButton5Action => InputHandler.MouseButton5Action;

    public LevelEditorController()
    {
        F1 = InputHandler.CreateInputAction("F1");
        F2 = InputHandler.CreateInputAction("F2");
        F3 = InputHandler.CreateInputAction("F3");

        RightArrow = InputHandler.CreateInputAction("\u2192");
        UpArrow = InputHandler.CreateInputAction("\u2191");
        LeftArrow = InputHandler.CreateInputAction("\u2190");
        DownArrow = InputHandler.CreateInputAction("\u2193");

        Space = InputHandler.CreateInputAction("Space");
        Enter = InputHandler.CreateInputAction("Enter");

        ToggleFullScreen = InputHandler.CreateInputAction("Toggle Full Screen");
        Quit = InputHandler.CreateInputAction("Quit");

        InputHandler.ValidateInputActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        InputHandler.Bind(Keys.F1, F1);
        InputHandler.Bind(Keys.F2, F2);
        InputHandler.Bind(Keys.F3, F3);

        InputHandler.Bind(Keys.A, LeftArrow);
        InputHandler.Bind(Keys.W, UpArrow);
        InputHandler.Bind(Keys.D, RightArrow);
        InputHandler.Bind(Keys.S, DownArrow);

        InputHandler.Bind(Keys.Left, LeftArrow);
        InputHandler.Bind(Keys.Up, UpArrow);
        InputHandler.Bind(Keys.Right, RightArrow);
        InputHandler.Bind(Keys.Down, DownArrow);

        InputHandler.Bind(Keys.Space, Space);
        InputHandler.Bind(Keys.Enter, Enter);

        InputHandler.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
        InputHandler.Bind(Keys.Escape, Quit);
    }

    public void ClearAllInputActions() => InputHandler.ClearAllInputActions();

    public void Tick() => InputHandler.Tick();
}
