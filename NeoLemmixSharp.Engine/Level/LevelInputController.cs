using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelInputController : InputController
{
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
        Pause = CreateKeyAction("Pause");
        Quit = CreateKeyAction("Quit");
        ToggleFullScreen = CreateKeyAction("Toggle Fullscreen");
        ToggleFastForwards = CreateKeyAction("Toggle Fast Forwards");
        SelectOnlyWalkers = CreateKeyAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = CreateKeyAction("Select Only Unassigned Lemmings");
        SelectLeftFacingLemmings = CreateKeyAction("Select Left Facing Lemmings");
        SelectRightFacingLemmings = CreateKeyAction("Select Right Facing Lemmings");

        RightArrow = CreateKeyAction("ABC");
        UpArrow = CreateKeyAction("ABC");
        LeftArrow = CreateKeyAction("ABC");
        DownArrow = CreateKeyAction("ABC");

        W = CreateKeyAction("ABC");
        A = CreateKeyAction("ABC");
        S = CreateKeyAction("ABC");
        D = CreateKeyAction("ABC");

        Space = CreateKeyAction("Space");

        ValidateKeyActions();

        SetUpBindings();
    }

    private void SetUpBindings()
    {
        Bind(Keys.P, Pause);
        Bind(Keys.Escape, Quit);
        Bind(Keys.F1, ToggleFullScreen);
        Bind(Keys.F, ToggleFastForwards);

        Bind(Keys.LeftControl, SelectOnlyUnassignedLemmings);
        Bind(Keys.W, SelectOnlyWalkers);

        Bind(Keys.Left, SelectLeftFacingLemmings);
        Bind(Keys.Right, SelectRightFacingLemmings);

        Bind(Keys.W, W);
        Bind(Keys.A, A);
        Bind(Keys.S, S);
        Bind(Keys.D, D);

        //Bind(Keys.Left, LeftArrow);
        Bind(Keys.Up, UpArrow);
        //Bind(Keys.Right, RightArrow);
        Bind(Keys.Down, DownArrow);

        Bind(Keys.Space, Space);
    }
}