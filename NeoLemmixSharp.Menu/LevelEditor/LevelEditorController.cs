using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed class LevelEditorController
{
    private readonly InputHandler _inputHandler;

    public Point MousePosition => _inputHandler.MousePosition;
    public int ScrollDelta => _inputHandler.ScrollDelta;

    public InputAction LeftMouseButtonAction => _inputHandler.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => _inputHandler.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => _inputHandler.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => _inputHandler.MouseButton4Action;
    public InputAction MouseButton5Action => _inputHandler.MouseButton5Action;

    public InputAction DownArrow { get; }
    public InputAction LeftArrow { get; }
    public InputAction UpArrow { get; }
    public InputAction RightArrow { get; }

    public InputAction DeleteSelection { get; }

    public InputAction ToggleFullScreen { get; }
    public InputAction Quit { get; }

    public LevelEditorController(InputHandler inputHandler)
    {
        inputHandler.ClearDefinedActions();

        _inputHandler = inputHandler;

        DownArrow = _inputHandler.CreateInputAction(EngineConstants.DownArrow);
        LeftArrow = _inputHandler.CreateInputAction(EngineConstants.LeftArrow);
        UpArrow = _inputHandler.CreateInputAction(EngineConstants.UpArrow);
        RightArrow = _inputHandler.CreateInputAction(EngineConstants.RightArrow);

        DeleteSelection = _inputHandler.CreateInputAction("Delete Selection");

        ToggleFullScreen = _inputHandler.CreateInputAction("Toggle Full Screen");
        Quit = _inputHandler.CreateInputAction("Quit");

        _inputHandler.ValidateInputActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        _inputHandler.Bind(Keys.A, LeftArrow);
        _inputHandler.Bind(Keys.W, UpArrow);
        _inputHandler.Bind(Keys.D, RightArrow);
        _inputHandler.Bind(Keys.S, DownArrow);

        _inputHandler.Bind(Keys.Left, LeftArrow);
        _inputHandler.Bind(Keys.Up, UpArrow);
        _inputHandler.Bind(Keys.Right, RightArrow);
        _inputHandler.Bind(Keys.Down, DownArrow);

        _inputHandler.Bind(Keys.Delete, DeleteSelection);
        _inputHandler.Bind(Keys.Back, DeleteSelection);

        _inputHandler.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
        _inputHandler.Bind(Keys.Escape, Quit);
    }
}
