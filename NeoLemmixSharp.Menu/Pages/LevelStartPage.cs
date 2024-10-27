using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : PageBase
{
    private readonly Panel _root;

    public LevelStartPage(
        MenuInputController inputController,
        LevelScreen levelScreen,
        LevelData levelData)
        : base(inputController)
    {
        _root = new Panel(Anchor.CenterLeft, new Vector2(80, 100), Vector2.Zero, false, true);

        UiRoot.AddChild(_root);
    }

    protected override void OnInitialise()
    {
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        _root.Size = new Vector2(windowWidth / 2, windowHeight);
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            NavigateToMainMenuPage();
        }
    }

    protected override void OnDispose()
    {
    }
}