using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Menu;

public sealed class MenuInputController
{
    private readonly InputController _inputController = new();

    public int MouseX => _inputController.MouseX;
    public int MouseY => _inputController.MouseY;
    public int ScrollDelta => _inputController.ScrollDelta;

    public MouseButtonAction LeftMouseButtonAction => _inputController.LeftMouseButtonAction;
    public MouseButtonAction RightMouseButtonAction => _inputController.RightMouseButtonAction;
    public MouseButtonAction MiddleMouseButtonAction => _inputController.MiddleMouseButtonAction;
    public MouseButtonAction MouseButton4Action => _inputController.MouseButton4Action;
    public MouseButtonAction MouseButton5Action => _inputController.MouseButton5Action;

    public KeyAction RightArrow { get; }
    public KeyAction UpArrow { get; }
    public KeyAction LeftArrow { get; }
    public KeyAction DownArrow { get; }

    public KeyAction Space { get; }
    public KeyAction Enter { get; }

    public KeyAction ToggleFullScreen { get; }
    public KeyAction Quit { get; }

    public MenuInputController()
    {
        RightArrow = _inputController.CreateKeyAction("\u2192");
        UpArrow = _inputController.CreateKeyAction("\u2191");
        LeftArrow = _inputController.CreateKeyAction("\u2190");
        DownArrow = _inputController.CreateKeyAction("\u2193");

        Space = _inputController.CreateKeyAction("Space");
        Enter = _inputController.CreateKeyAction("Enter");

        ToggleFullScreen = _inputController.CreateKeyAction("Toggle Full Screen");
        Quit = _inputController.CreateKeyAction("Quit");

        _inputController.ValidateKeyActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
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

        _inputController.Bind(Keys.F1, ToggleFullScreen);
        _inputController.Bind(Keys.Escape, Quit);
    }

    public void ClearAllKeys() => _inputController.ClearAllKeys();

    public void Tick() => _inputController.Tick();
}