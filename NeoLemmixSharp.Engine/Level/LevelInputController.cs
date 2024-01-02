using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.GameInput;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelInputController
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

	public KeyAction Pause { get; }
	public KeyAction Quit { get; }
	public KeyAction ToggleFullScreen { get; }
	public KeyAction ToggleFastForwards { get; }
	public KeyAction SelectOnlyWalkers { get; }
	public KeyAction SelectOnlyUnassignedLemmings { get; }
	public KeyAction SelectLeftFacingLemmings { get; }
	public KeyAction SelectRightFacingLemmings { get; }

	public KeyAction RightArrow { get; }
	public KeyAction UpArrow { get; }
	public KeyAction LeftArrow { get; }
	public KeyAction DownArrow { get; }

	public KeyAction W { get; }
	public KeyAction A { get; }
	public KeyAction S { get; }
	public KeyAction D { get; }

	public KeyAction Space { get; }

	public LevelInputController()
	{
		Pause = _inputController.CreateKeyAction("Pause");
		Quit = _inputController.CreateKeyAction("Quit");
		ToggleFullScreen = _inputController.CreateKeyAction("Toggle Fullscreen");
		ToggleFastForwards = _inputController.CreateKeyAction("Toggle Fast Forwards");
		SelectOnlyWalkers = _inputController.CreateKeyAction("Select Only Walkers");
		SelectOnlyUnassignedLemmings = _inputController.CreateKeyAction("Select Only Unassigned Lemmings");
		SelectLeftFacingLemmings = _inputController.CreateKeyAction("Select Left Facing Lemmings");
		SelectRightFacingLemmings = _inputController.CreateKeyAction("Select Right Facing Lemmings");

		RightArrow = _inputController.CreateKeyAction("ABC");
		UpArrow = _inputController.CreateKeyAction("ABC");
		LeftArrow = _inputController.CreateKeyAction("ABC");
		DownArrow = _inputController.CreateKeyAction("ABC");

		W = _inputController.CreateKeyAction("ABC");
		A = _inputController.CreateKeyAction("ABC");
		S = _inputController.CreateKeyAction("ABC");
		D = _inputController.CreateKeyAction("ABC");

		Space = _inputController.CreateKeyAction("Space");

		_inputController.ValidateKeyActions();

		SetUpBindings();
	}

	private void SetUpBindings()
	{
		_inputController.Bind(Keys.P, Pause);
		_inputController.Bind(Keys.Escape, Quit);
		_inputController.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
		_inputController.Bind(Keys.F, ToggleFastForwards);

		_inputController.Bind(Keys.LeftControl, SelectOnlyUnassignedLemmings);
		_inputController.Bind(Keys.W, SelectOnlyWalkers);

		_inputController.Bind(Keys.Left, SelectLeftFacingLemmings);
		_inputController.Bind(Keys.Right, SelectRightFacingLemmings);

		_inputController.Bind(Keys.W, W);
		_inputController.Bind(Keys.A, A);
		_inputController.Bind(Keys.S, S);
		_inputController.Bind(Keys.D, D);

		_inputController.Bind(Keys.Up, UpArrow);
		_inputController.Bind(Keys.Down, DownArrow);

		_inputController.Bind(Keys.Space, Space);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Tick() => _inputController.Tick();
}