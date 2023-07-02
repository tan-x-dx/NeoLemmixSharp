using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.GameInput;
using NeoLemmixSharp.Engine.FacingDirections;

namespace NeoLemmixSharp.Engine.Input;

public sealed class LevelInputController : InputController
{
    private int _actionCount;

    public SimpleKeyAction Pause { get; }
    public SimpleKeyAction Quit { get; }
    public SimpleKeyAction ToggleFullScreen { get; }
    public SimpleKeyAction ToggleFastForwards { get; }
    public SimpleKeyAction SelectOnlyWalkers { get; }
    public SimpleKeyAction SelectOnlyUnassignedLemmings { get; }
    public DirectionControlKeyAction SelectLeftFacingLemmings { get; }
    public DirectionControlKeyAction SelectRightFacingLemmings { get; }

    public LevelInputController()
        : base(8)
    {
        Pause = CreateSimpleAction("Pause");
        Quit = CreateSimpleAction("Quit");
        ToggleFullScreen = CreateSimpleAction("Toggle Fullscreen");
        ToggleFastForwards = CreateSimpleAction("Toggle Fast Forwards");
        SelectOnlyWalkers = CreateSimpleAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = CreateSimpleAction("Select Only Unassigned Lemmings");

        SelectLeftFacingLemmings = new DirectionControlKeyAction(_actionCount++, "Select Left Facing Lemmings", LeftFacingDirection.Instance);
        SelectRightFacingLemmings = new DirectionControlKeyAction(_actionCount++, "Select Right Facing Lemmings", RightFacingDirection.Instance);

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

        Bind(Keys.Left, SelectLeftFacingLemmings);
        Bind(Keys.Right, SelectRightFacingLemmings);
    }
}