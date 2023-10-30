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

    public MenuInputController()
    {
        RightArrow = _inputController.CreateKeyAction("\u2192");
        UpArrow = _inputController.CreateKeyAction("\u2191");
        LeftArrow = _inputController.CreateKeyAction("\u2190");
        DownArrow = _inputController.CreateKeyAction("\u2193");

        Space = _inputController.CreateKeyAction("Space");
        Enter = _inputController.CreateKeyAction("Enter");

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
    }
}