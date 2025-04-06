using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelInputController : IInitialisable
{
    private readonly InputController _inputController = new();

    public Point MousePosition => _inputController.MousePosition;
    public int ScrollDelta => _inputController.ScrollDelta;

    public InputAction LeftMouseButtonAction => _inputController.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => _inputController.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => _inputController.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => _inputController.MouseButton4Action;
    public InputAction MouseButton5Action => _inputController.MouseButton5Action;

    public InputAction Pause { get; }
    public InputAction Quit { get; }
    public InputAction ToggleFullScreen { get; }
    public InputAction ToggleFastForwards { get; }
    public InputAction SelectOnlyWalkers { get; }
    public InputAction SelectOnlyUnassignedLemmings { get; }
    public InputAction SelectLeftFacingLemmings { get; }
    public InputAction SelectRightFacingLemmings { get; }

    public InputAction Rewind50Frames { get; }
    public InputAction Reset { get; }

    public InputAction RightArrow { get; }
    public InputAction UpArrow { get; }
    public InputAction LeftArrow { get; }
    public InputAction DownArrow { get; }

    public InputAction W { get; }
    public InputAction A { get; }
    public InputAction S { get; }
    public InputAction D { get; }

    public InputAction Space { get; }

    public LevelInputController()
    {
        Pause = _inputController.CreateInputAction("Pause");
        Quit = _inputController.CreateInputAction("Quit");
        ToggleFullScreen = _inputController.CreateInputAction("Toggle Fullscreen");
        ToggleFastForwards = _inputController.CreateInputAction("Toggle Fast Forwards");
        SelectOnlyWalkers = _inputController.CreateInputAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = _inputController.CreateInputAction("Select Only Unassigned Lemmings");
        SelectLeftFacingLemmings = _inputController.CreateInputAction("Select Left Facing Lemmings");
        SelectRightFacingLemmings = _inputController.CreateInputAction("Select Right Facing Lemmings");

        Rewind50Frames = _inputController.CreateInputAction("Rewind 10 frames");
        Reset = _inputController.CreateInputAction("Reset");

        RightArrow = _inputController.CreateInputAction("ABC");
        UpArrow = _inputController.CreateInputAction("ABC");
        LeftArrow = _inputController.CreateInputAction("ABC");
        DownArrow = _inputController.CreateInputAction("ABC");

        W = _inputController.CreateInputAction("ABC");
        A = _inputController.CreateInputAction("ABC");
        S = _inputController.CreateInputAction("ABC");
        D = _inputController.CreateInputAction("ABC");

        Space = _inputController.CreateInputAction("Space");

        _inputController.ValidateInputActions();

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

        _inputController.Bind(Keys.OemMinus, Rewind50Frames);
        _inputController.Bind(Keys.R, Reset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Tick() => _inputController.Tick();

    public void Initialise()
    {
        SetEnabledWithFlag(Pause, LevelParameters.EnablePause);
        SetEnabledWithFlag(ToggleFastForwards, LevelParameters.EnableFastForward);
        SetEnabledWithFlag(SelectLeftFacingLemmings, LevelParameters.EnableDirectionSelect);
        SetEnabledWithFlag(SelectRightFacingLemmings, LevelParameters.EnableDirectionSelect);

        return;

        static void SetEnabledWithFlag(InputAction inputAction, LevelParameters testFlag)
        {
            inputAction.SetEnabled(LevelScreen.LevelParameters.Contains(testFlag));
        }
    }
}