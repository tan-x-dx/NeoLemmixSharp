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
        Pause = CreateAction();
        Quit = CreateAction();
        ToggleFullScreen = CreateAction();
        ToggleFastForwards = CreateAction();
        SelectOnlyWalkers = CreateAction();
        SelectOnlyUnassignedLemmings = CreateAction();

        SetUpBindings();
    }

    private KeyAction CreateAction()
    {
        return new KeyAction(_actionCount++);
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