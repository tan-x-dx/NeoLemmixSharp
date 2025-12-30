using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelInputController : IInitialisable
{
    public InputController InputController { get; } = new();

    public Point MousePosition => InputController.MousePosition;
    public int ScrollDelta => InputController.ScrollDelta;

    public InputAction LeftMouseButtonAction => InputController.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => InputController.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => InputController.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => InputController.MouseButton4Action;
    public InputAction MouseButton5Action => InputController.MouseButton5Action;

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
        Pause = InputController.CreateInputAction("Pause");
        Quit = InputController.CreateInputAction("Quit");
        ToggleFullScreen = InputController.CreateInputAction("Toggle Fullscreen");
        ToggleFastForwards = InputController.CreateInputAction("Toggle Fast Forwards");
        SelectOnlyWalkers = InputController.CreateInputAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = InputController.CreateInputAction("Select Only Unassigned Lemmings");
        SelectLeftFacingLemmings = InputController.CreateInputAction("Select Left Facing Lemmings");
        SelectRightFacingLemmings = InputController.CreateInputAction("Select Right Facing Lemmings");

        Rewind50Frames = InputController.CreateInputAction("Rewind 10 frames");
        Reset = InputController.CreateInputAction("Reset");

        RightArrow = InputController.CreateInputAction("ABC");
        UpArrow = InputController.CreateInputAction("ABC");
        LeftArrow = InputController.CreateInputAction("ABC");
        DownArrow = InputController.CreateInputAction("ABC");

        W = InputController.CreateInputAction("ABC");
        A = InputController.CreateInputAction("ABC");
        S = InputController.CreateInputAction("ABC");
        D = InputController.CreateInputAction("ABC");

        Space = InputController.CreateInputAction("Space");

        InputController.ValidateInputActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        InputController.Bind(Keys.P, Pause);
        InputController.Bind(Keys.Escape, Quit);
        InputController.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
        InputController.Bind(Keys.F, ToggleFastForwards);

        InputController.Bind(Keys.LeftControl, SelectOnlyUnassignedLemmings);
        InputController.Bind(Keys.W, SelectOnlyWalkers);

        InputController.Bind(Keys.Left, SelectLeftFacingLemmings);
        InputController.Bind(Keys.Right, SelectRightFacingLemmings);

        InputController.Bind(Keys.W, W);
        InputController.Bind(Keys.A, A);
        InputController.Bind(Keys.S, S);
        InputController.Bind(Keys.D, D);

        InputController.Bind(Keys.Up, UpArrow);
        InputController.Bind(Keys.Down, DownArrow);

        InputController.Bind(Keys.Space, Space);

        InputController.Bind(Keys.OemMinus, Rewind50Frames);
        InputController.Bind(Keys.R, Reset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Tick() => InputController.Tick();

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