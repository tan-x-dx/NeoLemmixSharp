using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine;

public sealed class LevelInputController : InputController
{
    private int _actionCount;

    public KeyAction Pause { get; }
    public KeyAction Quit { get; }
    public KeyAction ToggleFullScreen { get; }
    public KeyAction ToggleFastForwards { get; }
    public KeyAction SelectOnlyWalkers { get; }
    public KeyAction SelectOnlyUnassignedLemmings { get; }

    public LevelInputController()
        : base(6)
    {
        Pause = CreateAction("Pause");
        Quit = CreateAction("Quit");
        ToggleFullScreen = CreateAction("Toggle Fullscreen");
        ToggleFastForwards = CreateAction("Toggle Fast Forwards");
        SelectOnlyWalkers = CreateAction("Select Only Walkers");
        SelectOnlyUnassignedLemmings = CreateAction("Select Only Unassigned Lemmings");

        SetUpBindings();
    }

    private KeyAction CreateAction(string actionName)
    {
        return new KeyAction(_actionCount++, actionName);
    }

    private void SetUpBindings()
    {
        Bind(Keys.P, Pause);
        Bind(Keys.Escape, Quit);
        Bind(Keys.F1, ToggleFullScreen);
        Bind(Keys.F, ToggleFastForwards);

        Bind(Keys.LeftControl, SelectOnlyUnassignedLemmings);
        Bind(Keys.W, SelectOnlyWalkers);
    }
}