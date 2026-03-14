using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.GameInput;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelInputController : IInitialisable
{
    public InputHandler InputHandler { get; } = new();

    public Point MousePosition => InputHandler.MousePosition;
    public int ScrollDelta => InputHandler.ScrollDelta;

    public InputAction LeftMouseButtonAction => InputHandler.LeftMouseButtonAction;
    public InputAction RightMouseButtonAction => InputHandler.RightMouseButtonAction;
    public InputAction MiddleMouseButtonAction => InputHandler.MiddleMouseButtonAction;
    public InputAction MouseButton4Action => InputHandler.MouseButton4Action;
    public InputAction MouseButton5Action => InputHandler.MouseButton5Action;

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
        Pause = InputHandler.CreateInputAction("Pause");
        Quit = InputHandler.CreateInputAction("Quit");
        ToggleFullScreen = InputHandler.CreateInputAction("Toggle Fullscreen");
        ToggleFastForwards = InputHandler.CreateInputAction("Toggle Fast Forwards");
        SelectOnlyWalkers = InputHandler.CreateInputAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = InputHandler.CreateInputAction("Select Only Unassigned Lemmings");
        SelectLeftFacingLemmings = InputHandler.CreateInputAction("Select Left Facing Lemmings");
        SelectRightFacingLemmings = InputHandler.CreateInputAction("Select Right Facing Lemmings");

        Rewind50Frames = InputHandler.CreateInputAction("Rewind 10 frames");
        Reset = InputHandler.CreateInputAction("Reset");

        RightArrow = InputHandler.CreateInputAction("ABC");
        UpArrow = InputHandler.CreateInputAction("ABC");
        LeftArrow = InputHandler.CreateInputAction("ABC");
        DownArrow = InputHandler.CreateInputAction("ABC");

        W = InputHandler.CreateInputAction("ABC");
        A = InputHandler.CreateInputAction("ABC");
        S = InputHandler.CreateInputAction("ABC");
        D = InputHandler.CreateInputAction("ABC");

        Space = InputHandler.CreateInputAction("Space");

        InputHandler.ValidateInputActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        InputHandler.Bind(Keys.P, Pause);
        InputHandler.Bind(Keys.Escape, Quit);
        InputHandler.Bind(EngineConstants.FullscreenKey, ToggleFullScreen);
        InputHandler.Bind(Keys.F, ToggleFastForwards);

        InputHandler.Bind(Keys.LeftControl, SelectOnlyUnassignedLemmings);
        InputHandler.Bind(Keys.W, SelectOnlyWalkers);

        InputHandler.Bind(Keys.Left, SelectLeftFacingLemmings);
        InputHandler.Bind(Keys.Right, SelectRightFacingLemmings);

        InputHandler.Bind(Keys.W, W);
        InputHandler.Bind(Keys.A, A);
        InputHandler.Bind(Keys.S, S);
        InputHandler.Bind(Keys.D, D);

        InputHandler.Bind(Keys.Up, UpArrow);
        InputHandler.Bind(Keys.Down, DownArrow);

        InputHandler.Bind(Keys.Space, Space);

        InputHandler.Bind(Keys.OemMinus, Rewind50Frames);
        InputHandler.Bind(Keys.R, Reset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Tick() => InputHandler.Tick();

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