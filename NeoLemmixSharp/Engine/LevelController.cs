using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine;

public sealed class LevelController : KeyController<LevelKeyboardAction>
{
    private int _actionCount;

    public LevelKeyboardAction Pause { get; }
    public LevelKeyboardAction Quit { get; }
    public LevelKeyboardAction ToggleFullScreen { get; }
    public LevelKeyboardAction ToggleFastForwards { get; }

    public LevelController()
    {
        Pause = CreateAction();
        Quit = CreateAction();
        ToggleFullScreen = CreateAction();
        ToggleFastForwards = CreateAction();

        SetUpBindings();
    }

    private LevelKeyboardAction CreateAction()
    {
        return new LevelKeyboardAction(_actionCount++);
    }

    private void SetUpBindings()
    {
        Bind(Keys.P, Pause);
        Bind(Keys.Escape, Quit);
        Bind(Keys.F1, ToggleFullScreen);
        Bind(Keys.F, ToggleFastForwards);
    }
}

public sealed class LevelKeyboardAction : IKeyAction
{
    public int Id { get; }

    public LevelKeyboardAction(int id)
    {
        Id = id;
    }
}
