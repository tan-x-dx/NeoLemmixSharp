using MonoGameGum.GueDeriving;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : PageBase
{
    public LevelStartPage(
        MenuInputController inputController,
        LevelScreen levelScreen,
        LevelData levelData,
        ContainerRuntime root)
        : base(inputController, root)
    {
    }

    protected override void OnInitialise(ContainerRuntime root)
    {
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
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