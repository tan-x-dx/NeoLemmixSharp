using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelInputController : IInitialisable
{
    private readonly InputHandler _inputHandler;

    public Point MousePosition => _inputHandler.MousePosition;
    public int ScrollDelta => _inputHandler.ScrollDelta;

    public InputAction LeftMouseButtonAction => _inputHandler.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => _inputHandler.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => _inputHandler.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => _inputHandler.MouseButton4Action;
    public InputAction MouseButton5Action => _inputHandler.MouseButton5Action;

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

    public InputAction DownArrow { get; }
    public InputAction LeftArrow { get; }
    public InputAction UpArrow { get; }
    public InputAction RightArrow { get; }

    public InputAction W { get; }
    public InputAction A { get; }
    public InputAction S { get; }
    public InputAction D { get; }

    public InputAction Space { get; }

    public LevelInputController(InputHandler inputHandler)
    {
        _inputHandler = inputHandler;

        Pause = _inputHandler.CreateInputAction("Pause");
        Quit = _inputHandler.CreateInputAction("Quit");
        ToggleFullScreen = _inputHandler.CreateInputAction("Toggle Fullscreen");
        ToggleFastForwards = _inputHandler.CreateInputAction("Toggle Fast Forwards");
        SelectOnlyWalkers = _inputHandler.CreateInputAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = _inputHandler.CreateInputAction("Select Only Unassigned Lemmings");
        SelectLeftFacingLemmings = _inputHandler.CreateInputAction("Select Left Facing Lemmings");
        SelectRightFacingLemmings = _inputHandler.CreateInputAction("Select Right Facing Lemmings");

        Rewind50Frames = _inputHandler.CreateInputAction("Rewind 10 frames");
        Reset = _inputHandler.CreateInputAction("Reset");

        DownArrow = _inputHandler.CreateInputAction(EngineConstants.DownArrow);
        LeftArrow = _inputHandler.CreateInputAction(EngineConstants.LeftArrow);
        UpArrow = _inputHandler.CreateInputAction(EngineConstants.UpArrow);
        RightArrow = _inputHandler.CreateInputAction(EngineConstants.RightArrow);

        W = _inputHandler.CreateInputAction("ABC");
        A = _inputHandler.CreateInputAction("ABC");
        S = _inputHandler.CreateInputAction("ABC");
        D = _inputHandler.CreateInputAction("ABC");

        Space = _inputHandler.CreateInputAction("Space");

        _inputHandler.ValidateInputActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        _inputHandler.Bind(Keys.P, Pause);
        _inputHandler.Bind(Keys.Escape, Quit);
        _inputHandler.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
        _inputHandler.Bind(Keys.F, ToggleFastForwards);

        _inputHandler.Bind(Keys.LeftControl, SelectOnlyUnassignedLemmings);
        _inputHandler.Bind(Keys.W, SelectOnlyWalkers);

        _inputHandler.Bind(Keys.Left, SelectLeftFacingLemmings);
        _inputHandler.Bind(Keys.Right, SelectRightFacingLemmings);

        _inputHandler.Bind(Keys.W, W);
        _inputHandler.Bind(Keys.A, A);
        _inputHandler.Bind(Keys.S, S);
        _inputHandler.Bind(Keys.D, D);

        _inputHandler.Bind(Keys.Up, UpArrow);
        _inputHandler.Bind(Keys.Down, DownArrow);

        _inputHandler.Bind(Keys.Space, Space);

        _inputHandler.Bind(Keys.OemMinus, Rewind50Frames);
        _inputHandler.Bind(Keys.R, Reset);
    }

    public void Initialise()
    {
        SetEnabledWithFlag(Pause, LevelParameters.EnablePause);
        SetEnabledWithFlag(ToggleFastForwards, LevelParameters.EnableFastForward);
        SetEnabledWithFlag(SelectLeftFacingLemmings, LevelParameters.EnableDirectionSelect);
        SetEnabledWithFlag(SelectRightFacingLemmings, LevelParameters.EnableDirectionSelect);

        return;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SetEnabledWithFlag(InputAction inputAction, LevelParameters testFlag)
        {
            inputAction.SetEnabled(LevelScreen.LevelParameters.Contains(testFlag));
        }
    }
}