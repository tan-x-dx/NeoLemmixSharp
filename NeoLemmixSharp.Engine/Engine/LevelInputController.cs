using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Common.Util.GameInput;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class LevelInputController : InputController
{
    public KeyAction Pause { get; } = new("Pause");
    public KeyAction Quit { get; } = new("Quit");
    public KeyAction ToggleFullScreen { get; } = new("Toggle Fullscreen");
    public KeyAction ToggleFastForwards { get; } = new("Toggle Fast Forwards");
    public KeyAction SelectOnlyWalkers { get; } = new("Select Only Walkers");
    public KeyAction SelectOnlyUnassignedLemmings { get; } = new("Select Only Unassigned Lemmings");
    public KeyAction SelectLeftFacingLemmings { get; } = new("Select Left Facing Lemmings");
    public KeyAction SelectRightFacingLemmings { get; } = new("Select Right Facing Lemmings");

    public KeyAction RightArrow { get; } = new("ABC");
    public KeyAction UpArrow { get; } = new("ABC");
    public KeyAction LeftArrow { get; } = new("ABC");
    public KeyAction DownArrow { get; } = new("ABC");

    public KeyAction W { get; } = new("ABC");
    public KeyAction A { get; } = new("ABC");
    public KeyAction S { get; } = new("ABC");
    public KeyAction D { get; } = new("ABC");

    protected override void SetUpBindings()
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
    }
}