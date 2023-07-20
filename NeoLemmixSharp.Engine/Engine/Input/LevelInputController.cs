using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Engine.Engine.Input;

public sealed class LevelInputController : InputController
{
    private int _actionCount;

    public SimpleKeyAction Pause { get; }
    public SimpleKeyAction Quit { get; }
    public SimpleKeyAction ToggleFullScreen { get; }
    public SimpleKeyAction ToggleFastForwards { get; }
    public SimpleKeyAction SelectOnlyWalkers { get; }
    public SimpleKeyAction SelectOnlyUnassignedLemmings { get; }
    public SimpleKeyAction SelectLeftFacingLemmings { get; }
    public SimpleKeyAction SelectRightFacingLemmings { get; }

    public SimpleKeyAction RightArrow { get; }
    public SimpleKeyAction UpArrow { get; }
    public SimpleKeyAction LeftArrow { get; }
    public SimpleKeyAction DownArrow { get; }

    public SimpleKeyAction W { get; }
    public SimpleKeyAction A { get; }
    public SimpleKeyAction S { get; }
    public SimpleKeyAction D { get; }

    public LevelInputController()
        : base(14)
    {
        Pause = CreateSimpleAction("Pause");
        Quit = CreateSimpleAction("Quit");
        ToggleFullScreen = CreateSimpleAction("Toggle Fullscreen");
        ToggleFastForwards = CreateSimpleAction("Toggle Fast Forwards");
        SelectOnlyWalkers = CreateSimpleAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = CreateSimpleAction("Select Only Unassigned Lemmings");

        RightArrow = CreateSimpleAction("ABC");
        UpArrow = CreateSimpleAction("ABC");
        LeftArrow = CreateSimpleAction("ABC");
        DownArrow = CreateSimpleAction("ABC");
        W = CreateSimpleAction("ABC");
        A = CreateSimpleAction("ABC");
        S = CreateSimpleAction("ABC");
        D = CreateSimpleAction("ABC");

        SelectLeftFacingLemmings = CreateSimpleAction("Select Left Facing Lemmings");
        SelectRightFacingLemmings = CreateSimpleAction("Select Right Facing Lemmings");

        SetUpBindings();

        SimpleKeyAction CreateSimpleAction(string actionName)
        {
            return new SimpleKeyAction(_actionCount++, actionName);
        }
    }

    private void SetUpBindings()
    {
        Bind(Keys.P, Pause);
        Bind(Keys.Escape, Quit);
        Bind(Keys.F1, ToggleFullScreen);
        Bind(Keys.F, ToggleFastForwards);

        Bind(Keys.LeftControl, SelectOnlyUnassignedLemmings);
        Bind(Keys.W, SelectOnlyWalkers);

        //  Bind(Keys.Left, SelectLeftFacingLemmings);
        //  Bind(Keys.Right, SelectRightFacingLemmings);

        Bind(Keys.W, W);
        Bind(Keys.A, A);
        Bind(Keys.S, S);
        Bind(Keys.D, D);

        Bind(Keys.Left, LeftArrow);
        Bind(Keys.Up, UpArrow);
        Bind(Keys.Right, RightArrow);
        Bind(Keys.Down, DownArrow);
    }
}